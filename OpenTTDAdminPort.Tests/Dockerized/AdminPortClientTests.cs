#pragma warning disable // will be enabled later TODO
using System;
using System.Threading.Tasks;

using Divergic.Logging.Xunit;

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

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class AdminPortClientTests : DockerizedTest<OpenttdServerContainer>
    {
        private readonly ILoggerFactory loggerFactory;

        public AdminPortClientTests(ITestOutputHelper output)
            : base(output)
        {
        }

        protected override void SetupServiceProvider(IServiceCollection services)
        {
            base.SetupServiceProvider(services);
        }

        [Fact]
        public async Task PingPongTest()
        {
            await application.Start();
            AdminPongEvent pongEvent = null;
            AdminPortClient client = new AdminPortClient(AdminPortClientSettings.Default, application.ServerInfo, builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(output));
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

        [Fact]
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
                builder.AddProvider(new XUnitLoggerProvider(output));
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            AdminPongEvent pongEvent = null;
            AdminServerConnected connectEvent = null;

            client.SetAdminEventHandler(ev =>
            {
                if (ev is AdminPongEvent pe)
                {
                    pongEvent = pe;
                }
                if (ev is AdminServerConnected pr)
                {
                    connectEvent = pr;
                }
            });

            logger.LogInformation("Starting client connection");
            await client.Connect();
            logger.LogInformation("Client connected");

            logger.LogInformation("Starting openttd server stop");
            await application.Stop();
            logger.LogInformation("openttd stopped");

            await application.Start();

            logger.LogInformation("Server started again");

            logger.LogInformation("Waiting to receive message about restart");
            var timeout = Task.Delay(settings.WatchdogInterval * 3 + 30.Seconds());
            connectEvent = null;

            while (connectEvent == null)
            {
                await Task.Delay(1);

                if (timeout.IsCompleted)
                    throw new Exception();
            }
            logger.LogInformation("Restart ocurred");

            logger.LogInformation("Sending ping");
            client.SendMessage(new AdminPingMessage(22u));

            timeout = Task.Delay(3.Seconds());

            while (pongEvent?.PongValue != 22u)
            {
                await Task.Delay(1);

                if (timeout.IsCompleted)
                {
                    throw new Exception();
                }
            }

            Assert.Equal(22u, pongEvent.PongValue);
        }
    }
}
