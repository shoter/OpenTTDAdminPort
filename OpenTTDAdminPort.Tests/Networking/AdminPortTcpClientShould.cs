using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.Tests.Logging;

using System;
using System.IO;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientShould : BaseTestKit
    {
        private readonly TestProbe probe;

        private readonly DummyTcpClient tcpClient = new DummyTcpClient();

        private readonly IServiceProvider defaultServiceProvider;

        private readonly Mock<NetworkingActorFactory> actorFactory;

        private readonly Fixture fix = new();



        public AdminPortTcpClientShould(ITestOutputHelper testOutputHelper)
        {
            probe = CreateTestProbe();


            defaultServiceProvider = new ServiceCollection()
                .AddSingleton<ITcpClient>(tcpClient)
                .AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create())
                .AddSingleton<IActorFactory, ActorFactory>()
                .AddSingleton<INetworkingActorFactory>(sp => actorFactory.Object)
                .AddLogging(logging =>
                {
                    logging.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .BuildServiceProvider();

            actorFactory = new(defaultServiceProvider);
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

        [Fact]
        public void BeAbleToSendMessage()
        {
            uint value = fix.Create<uint>();

            var actor = Sys.ActorOf(AdminPortTcpClient.Create(defaultServiceProvider, "", 0));
            var msg = new AdminPingMessage(value);
            actor.Tell(new SendMessage(msg));

            // TODO: Think of better way of testing that
            // Something was written
            WaitUntil(() => tcpClient.Stream.Length > 0);
        }

        [Fact]
        public void SendReceivedMessageToParent()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();


            IActorRef actor = probe.ChildActorOf(AdminPortTcpClient.Create(defaultServiceProvider, "", 0));
            // TODO: Think of better way of testing that
            // Something was written
            Within(3.Seconds(), () =>
            {
                actor.Tell(new ReceiveMessage(msg));

                probe.ExpectMsg((ReceiveMessage rec) =>
                    rec.Message == msg
                    );
            });
        }
    }
}
