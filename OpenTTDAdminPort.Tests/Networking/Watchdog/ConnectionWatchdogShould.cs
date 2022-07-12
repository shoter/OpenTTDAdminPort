using System;

using Akka.Actor;
using Akka.TestKit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Watchdog;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Networking.Watchdog
{
    public class ConnectionWatchdogShould : BaseTestKit
    {
        private readonly TimeSpan pingTime = 1.Seconds();

        public ConnectionWatchdogShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        protected override void ConfigureServiceCollection(IServiceCollection services)
        {
            base.ConfigureServiceCollection(services);

            services.Configure<AdminPortClientSettings>(x =>
            {
                x.WatchdogInterval = pingTime;
            });
        }

        [Fact]
        public void AutomaticallySendPingMessage_AfterSpecifiedTimeSpan()
        {
            IActorRef dog = ActorOf(ConnectionWatchdog.Create(defaultServiceProvider, tcpClient: probe.Ref, pingTime));
            probe.ExpectMsg<TcpClientSubscribe>();

            probe.Within(pingTime * 2, () =>
            {
                probe.ExpectMsg<SendMessage>(msg => msg.Message is AdminPingMessage);
            });
        }

        [Fact]
        public void DoNothing_WhenReceivesResponseInTime()
        {
            TestProbe parent = CreateTestProbe();
            IActorRef dog = parent.ChildActorOf(ConnectionWatchdog.Create(defaultServiceProvider, tcpClient: probe.Ref, pingTime));
            probe.ExpectMsg<TcpClientSubscribe>();

            for (int i = 0; i < 5; ++i)
            {
                probe.Within(pingTime * 3, () =>
                {
                    uint arg = 0;
                    probe.ExpectMsg<SendMessage>(msg =>
                    {
                        if (msg.Message is AdminPingMessage ping)
                        {
                            arg = ping.Argument;
                            return true;
                        }

                        return false;
                    });

                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg)));
                });
            }

            // Parent should receive nothing through whole test.
            parent.ExpectNoMsg(1.Millis());
        }

        [Fact]
        public void DoNothing_WhenReceivesWrongPongs_AndFinallyReceivesCorrectOne()
        {
            TestProbe parent = CreateTestProbe();
            IActorRef dog = parent.ChildActorOf(ConnectionWatchdog.Create(defaultServiceProvider, tcpClient: probe.Ref, pingTime));
            probe.ExpectMsg<TcpClientSubscribe>();

            for (int i = 0; i < 5; ++i)
            {
                probe.Within(pingTime * 2, () =>
                {
                    uint arg = 0;
                    probe.ExpectMsg<SendMessage>(msg =>
                    {
                        if (msg.Message is AdminPingMessage ping)
                        {
                            arg = ping.Argument;
                            return true;
                        }

                        return false;
                    });

                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 1)));
                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 2)));
                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 3)));
                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 4)));
                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 5)));
                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg + 6)));

                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg)));
                });
            }

            // Parent should receive nothing through whole test.
            parent.ExpectNoMsg(1.Millis());
        }

        [Fact]
        public void ErrorOut_WHenDOesNotReceiveReply_EvenAfterSuccessfullReplies()
        {
            TestProbe parent = CreateTestProbe();
            IActorRef dog = parent.ChildActorOf(ConnectionWatchdog.Create(defaultServiceProvider, tcpClient: probe.Ref, pingTime));
            probe.ExpectMsg<TcpClientSubscribe>();

            for (int i = 0; i < 5; ++i)
            {
                probe.Within(pingTime * 3, () =>
                {
                    uint arg = 0;
                    probe.ExpectMsg<SendMessage>(msg =>
                    {
                        if (msg.Message is AdminPingMessage ping)
                        {
                            arg = ping.Argument;
                            return true;
                        }

                        return false;
                    });

                    probe.Reply(new ReceiveMessage(new AdminServerPongMessage(arg)));
                });
            }

            // Parent should receive nothing through whole test.
            parent.ExpectMsg<WatchdogConnectionLost>(pingTime * 3);
        }

        [Fact]
        public void ErrorOut_WhenDoesNotReceiveReply_InLongEnoughTime()
        {
            TestProbe parent = CreateTestProbe();
            IActorRef dog = parent.ChildActorOf(ConnectionWatchdog.Create(defaultServiceProvider, tcpClient: probe.Ref, pingTime));
            probe.ExpectMsg<TcpClientSubscribe>();

            Within(pingTime * 3, () =>
            {
                parent.ExpectMsg<WatchdogConnectionLost>();
            });
        }
    }
}
