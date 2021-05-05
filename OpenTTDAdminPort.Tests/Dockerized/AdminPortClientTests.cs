using Divergic.Logging.Xunit;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
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
    public class AdminPortClientTests : DockerizedTest
    {
        private readonly ILoggerFactory loggerFactory;
        public AdminPortClientTests(ITestOutputHelper output)
        {
            loggerFactory = LogFactory.Create(output);

        }
        [Fact]
        public Task PingPongTest() => StartTest(nameof(PingPongTest), async () =>
            {
                AdminPortClient client = new AdminPortClient(ServerInfo, loggerFactory.CreateLogger<AdminPortClient>());

                AdminPongEvent pongEvent = null;
                client.EventReceived += (_, e) =>
                {
                    if (e is AdminPongEvent pe)
                        pongEvent = pe;
                };

                await client.Connect();
                client.SendMessage(new AdminPingMessage(55));

                while (pongEvent == null)
                {
                    await Task.Delay(1);
                }

                Assert.Equal(55u, pongEvent.PongValue);
                await client.Disconnect();
            });

    }
}
