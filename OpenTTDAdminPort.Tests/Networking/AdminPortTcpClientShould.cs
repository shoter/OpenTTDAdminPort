using Akka.TestKit;
using Akka.TestKit.Xunit2;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.Tests.Logging;

using System;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientShould : BaseTestKit
    {
        private readonly TestProbe probe;

        private readonly DummyTcpClient tcpClient = new DummyTcpClient();

        private readonly IServiceProvider defaultServiceProvider;

        private readonly Fixture fix = new();



        public AdminPortTcpClientShould(ITestOutputHelper testOutputHelper)
        {
            probe = CreateTestProbe();


            defaultServiceProvider = new ServiceCollection()
                .AddSingleton<ITcpClient>(tcpClient)
                .AddSingleton<IAdminPacketService, AdminPacketService>()
                .AddSingleton<IActorFactory, ActorFactory>()
                .AddLogging(logging =>
                {
                    logging.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .BuildServiceProvider();
        }

        [Fact]
        public void AutomaticallyConnectToServer()
        {
            string ip = fix.Create<string>();
            int port = Math.Abs(fix.Create<int>());
            var actor = Sys.ActorOf(AdminPortTcpClient.Create(defaultServiceProvider, ip, port));

            WaitUntil(() =>
            {
                return tcpClient.IsConnected;
            });

            Assert.Equal(ip, tcpClient.Ip);
            Assert.Equal(port, tcpClient.Port);
        }

    }
}
