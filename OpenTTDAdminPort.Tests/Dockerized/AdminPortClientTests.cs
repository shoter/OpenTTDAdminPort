using Divergic.Logging.Xunit;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Logging;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public AdminPortClientTests(ITestOutputHelper output)
        {
            loggerFactory = LogFactory.Create(output);
            loggerFactory.AddProvider(new DebugLoggerProvider());
            logger = loggerFactory.CreateLogger<AdminPortClientTests>();
        }

        [Fact]
        public async Task PingPongTest()
        {
            await server.Start(nameof(PingPongTest));
            AdminPortClient client = new AdminPortClient(server.ServerInfo, loggerFactory.CreateLogger<AdminPortClient>());

            AdminPongEvent pongEvent = null;
            client.EventReceived += (_, e) =>
            {
                if (e is AdminPongEvent pe)
                    pongEvent = pe;
            };

            await client.Connect();
            client.SendMessage(new AdminPingMessage(55u));

            while (pongEvent == null)
            {
                await Task.Delay(1);
            }

            Assert.Equal(55u, pongEvent.PongValue);
            await client.Disconnect();
        }

        //[Fact]
        //public async Task AfterServerRestart_AdminPortClientShouldAutomaticallyReconnect()
        //{
        //    logger.LogInformation("Starting Openttd server");
        //    await server.Start(nameof(PingPongTest));
        //    logger.LogInformation("Openttd Server started");
        //    AdminPortClient client = new AdminPortClient(server.ServerInfo, new ContextLogger<AdminPortClient>(loggerFactory.CreateLogger<AdminPortClient>(), "Main Test Client"));

        //    logger.LogInformation("Starting client connection");
        //    await client.Connect();
        //    logger.LogInformation("Client connected");

        //    logger.LogInformation("Starting openttd server stop");
        //    bool erroredOut = false;
        //    client.StateChanged += (_, arg) => erroredOut = erroredOut | arg.New == AdminConnectionState.Errored;
        //    await server.Stop();
        //    logger.LogInformation("openttd stopped");

        //    logger.LogInformation("Waiting for errored state");


        //    if (!(await TaskHelper.WaitUntil(() => erroredOut, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(20))))
        //    {
        //        throw new AdminPortException("Wrong State!");
        //    }
        //    logger.LogInformation("Errored state established");

        //    await server.Start(nameof(PingPongTest));

        //    logger.LogInformation("Openttd server started (again)");


        //    if (!(await TaskHelper.WaitUntil(() => client.ConnectionState == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(20))))
        //    {
        //        throw new AdminPortException("Wrong State!");
        //    }

        //    logger.LogInformation("Connected state established");

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
