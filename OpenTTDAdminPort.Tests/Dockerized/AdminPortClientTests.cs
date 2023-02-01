#pragma warning disable // will be enabled later TODO
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Logging;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Tests.Dockerized.Applications;
using OpenTTDAdminPort.Tests.Dockerized.Containers;
using OpenTTDAdminPort.Tests.Logging;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class AdminPortClientTests : DockerizedTest<OpenttdServerContainer>
    {
        private readonly ILoggerFactory loggerFactory;

        public AdminPortClientTests(ITestOutputHelper output)
            : base(output)
        {
            this.application.AdditionalBuilder = builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(output, "OTTD_SERVER"));
                builder.SetMinimumLevel(LogLevel.Trace);
            };
        }

        protected override void SetupServiceProvider(IServiceCollection services)
        {
            base.SetupServiceProvider(services);
        }

        [Fact (Timeout = 30_000, Skip = "test")]
        public async Task PingPongTest()
        {
            await application.Start();
            AdminPongEvent pongEvent = null;
            AdminPortClient client = new AdminPortClient(AdminPortClientSettings.Default, application.ServerInfo, builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(output, "SUT"));
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            client.SetAdminEventHandler(ev =>
            {
                if (ev is AdminPongEvent pe)
                {
                    pongEvent = pe;
                }
            });


            await client.Connect();
            client.SendMessage(new AdminPingMessage(55u));

            var timeout = Task.Delay(15.Seconds());

            while (pongEvent == null)
            {
                await Task.Delay(1);

                if (timeout.IsCompleted)
                {
                    throw new Exception();
                }
            }

            Assert.Equal(55u, pongEvent.PongValue);
            await client.Disconnect();
        }

        [Fact(Timeout = 60_000)]
        public async Task AfterServerRestart_AdminPortClientShouldAutomaticallyReconnect()
        {
            var settings = new AdminPortClientSettings()
            {
                WatchdogInterval = 1.Seconds()
            };
            logger.LogInformation("Starting Openttd server");

            await application.Start(nameof(AfterServerRestart_AdminPortClientShouldAutomaticallyReconnect));
            logger.LogInformation($"Openttd Server started on port {application.Port}");
            AdminPortClient client = new AdminPortClient(settings, application.ServerInfo, builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(output, "SUT"));
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            AdminPongEvent pongEvent = null;
            AdminServerConnectionLost connectionLostEvent = null;
            AdminServerConnected connectEvent = null;

            client.SetAdminEventHandler(ev =>
            {
                logger.LogTrace($"Received {ev}");
                if (ev is AdminPongEvent pe)
                {
                    if (pe.PongValue == 22u)
                    {
                        logger.LogTrace("Received pong message I was waiting for <3");
                        pongEvent = pe;
                    }
                }
                if (ev is AdminServerConnected pr)
                {
                    connectEvent = pr;
                }
                if(ev is AdminServerConnectionLost cle)
                {
                    connectionLostEvent = cle;
                }
            });

            logger.LogInformation("Starting client connection");
            await client.Connect();
            logger.LogInformation("Client connected");

            logger.LogInformation("Starting openttd server stop");
            await application.Stop();
            logger.LogInformation("openttd stopped");

            while (connectionLostEvent == null)
            {
                await Task.Delay(1);
            }

            logger.LogInformation("openttd stop detected - starting it again.");

            connectEvent = null;
            await application.Start(port: application.Port);

            logger.LogInformation("Server started again");

            logger.LogInformation("Waiting to receive message about restart");
            var timeout = Task.Delay(settings.WatchdogInterval * 3 + 30.Seconds());

            while (connectEvent == null)
            {
                await Task.Delay(1);

                if (timeout.IsCompleted)
                {
                    throw new Exception();
                }
            }
            logger.LogInformation("Restart ocurred");

            logger.LogInformation("Sending ping");
            pongEvent = null;
            client.SendMessage(new AdminPingMessage(22u));

            timeout = Task.Delay(3.Seconds());

            while (pongEvent?.PongValue != 22u)
            {
                await Task.Delay(1);
                logger.LogTrace($"{pongEvent?.PongValue}");

                if (timeout.IsCompleted)
                {
                    throw new Exception();
                }
            }

            Assert.Equal(22u, pongEvent.PongValue);
        }
    }
}
