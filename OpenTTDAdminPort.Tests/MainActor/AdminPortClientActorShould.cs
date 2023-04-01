using System;

using Akka.Actor;
using Akka.TestKit;

using Moq;

using OpenTTDAdminPort.MainActor;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.MainActor
{
    public class AdminPortClientActorShould : BaseTestKit
    {
        private readonly TestProbe tcpClient;

        private readonly TestProbe watchdog;

        public AdminPortClientActorShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            tcpClient = CreateTestProbe();
            watchdog = CreateTestProbe();

            actorFactoryMock.Setup(x => x.CreateTcpClient(It.IsAny<IActorContext>(), It.IsAny<string>(), It.IsAny<int>())).Returns(tcpClient.Ref);
            actorFactoryMock.Setup(x => x.CreateWatchdog(It.IsAny<IActorContext>(), tcpClient.Ref, It.IsAny<TimeSpan>())).Returns(watchdog.Ref);
        }

        [Fact]
        public void ShouldCreateTcpClientWithCorrectParameters_UponConnect()
        {
            string ip = "127.0.0.1";
            int port = 12345;

            var actor = Sys.ActorOf(AdminPortClientActor.Create(SP));

            Within(3.Seconds(), () =>
            {
                actor.Tell(new AdminPortConnect(new ServerInfo(ip, port), "Zenek"));
                AwaitAssert(() =>
                {
                    actorFactoryMock.Verify(x => x.CreateTcpClient(It.IsAny<IActorContext>(), ip, port), Times.Once());
                });
            });
        }

        [Fact]
        public void SendProperMessages_ToTcpClient_InOrderToJoin()
        {
            string ip = "127.0.0.1";
            string password = "StarWarsIsNiceMovie";
            int port = 12345;

            var actor = Sys.ActorOf(AdminPortClientActor.Create(SP));

            Within(3.Seconds(), () =>
            {
                actor.Tell(new AdminPortConnect(new ServerInfo(ip, port, password), "adminX"));
                tcpClient.ExpectMsg<SendMessage>(sm =>
                {
                    var m = (sm.Message as AdminJoinMessage)!;
                    return m.Password == password &&
                    m.AdminName == "adminX";
                });
            });
        }

        [Fact]
        public void ShouldBeAbleToDisconnect_AfterConnectMessage()
        {
            string ip = "127.0.0.1";
            int port = 12345;

            var actor = Sys.ActorOf(AdminPortClientActor.Create(SP));

            Within(10.Seconds(), () =>
            {
                actor.Tell(new AdminPortConnect(new ServerInfo(ip, port), "adminX"));
                actor.Tell(new AdminPortDisconnect());
                probe.Watch(tcpClient);
                probe.FishForMessage<Terminated>(x => x.ActorRef == tcpClient.Ref);
            });
        }
    }
}
