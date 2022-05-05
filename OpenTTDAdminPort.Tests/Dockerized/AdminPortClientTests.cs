using Divergic.Logging.Xunit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Logging;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Tests.Dockerized.Containers;
using OpenTTDAdminPort.Tests.Logging;

using System;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class AdminPortClientTests : IDisposable
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly OpenttdServerContainer server = new OpenttdServerContainer(DockerClientProvider.Instance);
        private bool disposedValue;
        private ILogger logger;
        ITestOutputHelper output;

        public AdminPortClientTests(ITestOutputHelper output)
        {
            this.output = output;
            loggerFactory = LogFactory.Create(output);
            loggerFactory.AddProvider(new DebugLoggerProvider());
            logger = loggerFactory.CreateLogger<AdminPortClientTests>();
        }

        [Fact]
        public async Task PingPongTest()
        {
            await server.Start(nameof(PingPongTest));
            AdminPongEvent pongEvent = null;
            AdminPortClient client = new AdminPortClient(AdminPortClientSettings.Default, server.ServerInfo, builder =>
            {
                builder.AddProvider(new DebugLoggerProvider());
                builder.AddProvider(new XUnitLoggerProvider(output));
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

            while (pongEvent == null)
            {
                await Task.Delay(1);
            }

            Assert.Equal(55u, pongEvent.PongValue);
            await client.Disconnect();
        }

        // This needs to be rewritten :(
        //[Fact]
        //public async Task AfterServerRestart_AdminPortClientShouldAutomaticallyReconnect()
        //{
        //    var settings = new AdminPortClientSettings()
        //    {
        //        WatchdogInterval = TimeSpan.FromSeconds(5),
        //    };
        //    logger.LogInformation("Starting Openttd server");
        //    await server.Start(nameof(PingPongTest));
        //    logger.LogInformation("Openttd Server started");
        //    AdminPortClient client = new AdminPortClient(settings, server.ServerInfo, builder =>
        //    {
        //        builder.AddProvider(new XUnitLoggerProvider(output));
        //    });

        //    AdminPongEvent pongEvent = null;
        //    AdminServerRestarted restartEvent = null;

        //    client.SetAdminEventHandler(ev =>
        //    {
        //        if (ev is AdminPongEvent pe)
        //        {
        //            pongEvent = pe;
        //        }
        //        if(ev is AdminServerRestarted pr)
        //        {
        //            restartEvent = pr;
        //        }
        //    });


        //    logger.LogInformation("Starting client connection");
        //    await client.Connect();
        //    logger.LogInformation("Client connected");

        //    logger.LogInformation("Starting openttd server stop");
        //    await server.Stop();
        //    logger.LogInformation("openttd stopped");

        //    await server.Start(nameof(PingPongTest));

        //    logger.LogInformation("Server started again");

        //    logger.LogInformation("Waiting to receive message about restart");
        //    var timeout = Task.Delay(3.Seconds());

        //    while (restartEvent == null)
        //    {
        //        await Task.Delay(1);

        //        if (timeout.IsCompleted)
        //            throw new Exception();
        //    }
        //    logger.LogInformation("Restart ocurred");

        //    logger.LogInformation("Sending ping");
        //    client.SendMessage(new AdminPingMessage(22u));

        //    timeout = Task.Delay(3.Seconds());

        //    while (pongEvent == null)
        //    {
        //        await Task.Delay(1);

        //        if (timeout.IsCompleted)
        //            throw new Exception();
        //    }

        //    Assert.Equal(22u, pongEvent.PongValue);
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    server.Dispose();
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
