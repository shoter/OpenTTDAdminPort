using Akka.Actor;
using Akka.Configuration;
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
using System.Linq;
using System.Reflection;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientShould : TestKit
    {
        private readonly TestProbe probe;

        private readonly DummyTcpClient tcpClient = new DummyTcpClient();

        private readonly IServiceProvider defaultServiceProvider;

        private readonly Mock<ActorFactory> actorFactory;

        private readonly Fixture fix = new();



        public AdminPortTcpClientShould(ITestOutputHelper testOutputHelper)
            : base(
                  ConfigurationFactory.ParseString("\r\n                akka.test.testkit.debug = true\r\n                akka.log-dead-letters-during-shutdown = true\r\n                akka.actor.debug.receive = true\r\n                akka.actor.debug.autoreceive = true\r\n                akka.actor.debug.lifecycle = true\r\n                akka.actor.debug.event-stream = true\r\n                akka.actor.debug.unhandled = true\r\n                akka.actor.debug.fsm = true\r\n                akka.actor.debug.router-misconfiguration = true\r\n                akka.log-dead-letters = true\r\n                akka.loglevel = DEBUG\r\n                akka.stdout-loglevel = DEBUG")
                  , null, testOutputHelper)
        {
            probe = CreateTestProbe();


            defaultServiceProvider = new ServiceCollection()
                .AddSingleton<ITcpClient>(tcpClient)
                .AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create())
                .AddSingleton<IActorFactory>(sp => actorFactory.Object)
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

            Within(1.Seconds(), () =>
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
            Within(1.Seconds(), () => tcpClient.Stream.Length > 0);
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

        [Fact]
        public void SendReceiveMessageToSubscriber()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();
            var someone = CreateTestProbe();


            IActorRef actor = probe.ChildActorOf(AdminPortTcpClient.Create(defaultServiceProvider, "", 0));
            actor.Tell(new TcpClientSubscribe(), someone);
            // TODO: Think of better way of testing that
            // Something was written
            Within(3.Seconds(), () =>
            {
                actor.Tell(new ReceiveMessage(msg));

                someone.ExpectMsg((ReceiveMessage rec) =>
                    rec.Message == msg
                    );
            });
        }

        [Fact]
        public void NotSendMessage_AfterUnsubscribe()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();
            var someone = CreateTestProbe();

            IActorRef actor = probe.ChildActorOf(AdminPortTcpClient.Create(defaultServiceProvider, "", 0));
            actor.Tell(new TcpClientSubscribe(), someone);

            // TODO: Think of better way of testing that
            // Something was written
            someone.Within(2.Seconds(), () =>
            {
                actor.Tell(new TcpClientUnsubscribe(), someone);
                actor.Tell(new ReceiveMessage(msg));

                someone.ExpectNoMsg();
            }, 1.Seconds());

        }

        [Fact]
        public void Unsubscribing_NonExistingClient_ShouldNotCauseAnyException()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();
            var someone = CreateTestProbe();

            IActorRef actor = probe.ActorOf(AdminPortTcpClient.Create(defaultServiceProvider, "", 0));
            // TODO: Think of better way of testing that
            // Something was written
            someone.Within(3.Seconds(), () =>
            {
                actor.Tell(new TcpClientUnsubscribe(), someone);
                actor.Tell(new TcpClientUnsubscribe(), someone);
                actor.Tell(new ReceiveMessage(msg));

                someone.ExpectNoMsg();

            }, 1.Seconds());

        }
    }
}
