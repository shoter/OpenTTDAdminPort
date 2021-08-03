using Moq;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortConnectedStateShould : BaseStateShould
    {
        AdminPortConnectedState state = new AdminPortConnectedState();

        public AdminPortConnectedStateShould()
        {
            context.State = AdminConnectionState.Connected;
        }

        [Fact]
        public void SendMessagesThatAreQueued_WhenStartingThisState()
        {
            List<IAdminMessage> queuedMessages = new List<IAdminMessage>();

            for (int i = 0; i < 100; ++i)
            {
                queuedMessages.Add(Mock.Of<IAdminMessage>());
                context.MessagesToSend.Enqueue(queuedMessages[i]);
            }

            state.OnStateStart(context);

            foreach (var msg in queuedMessages)
            {
                tcpClientMock.Verify(x => x.SendMessage(msg), Times.Once);
            }
        }

        [Fact]
        public void StartWatchdog_WhenStartingState()
        {
            var contextMock = new Mock<IAdminPortClientContext>();
            var dogMock = new Mock<IConnectionWatchdog>();
            var tcpClient = tcpClientMock.Object;
            contextMock.Setup(x => x.WatchDog).Returns(dogMock.Object);
            contextMock.Setup(x => x.TcpClient).Returns(tcpClientMock.Object);
            contextMock.SetupGet(x => x.MessagesToSend).Returns(new ConcurrentQueue<IAdminMessage>());
            state.OnStateStart(contextMock.Object);
            dogMock.Verify(x => x.Start(tcpClientMock.Object), Times.Once);
        }

        [Fact]
        public void StopWatchdog_WhenStateEnds()
        {
            var contextMock = new Mock<IAdminPortClientContext>();
            var dogMock = new Mock<IConnectionWatchdog>();
            contextMock.SetupGet(x => x.MessagesToSend).Returns(new ConcurrentQueue<IAdminMessage>());
            contextMock.Setup(x => x.WatchDog).Returns(dogMock.Object);
            contextMock.Setup(x => x.TcpClient).Returns(tcpClientMock.Object);
            state.OnStateEnd(contextMock.Object);
            dogMock.Verify(x => x.Stop(), Times.Once);
        }

        [Fact]
        public void ErrorsOut_WhenWatchDogThrowErrorAfterStart()
        {
            var contextMock = new Mock<IAdminPortClientContext>();
            var dogMock = new Mock<IConnectionWatchdog>();
            contextMock.SetupGet(x => x.MessagesToSend).Returns(new ConcurrentQueue<IAdminMessage>());
            contextMock.Setup(x => x.WatchDog).Returns(dogMock.Object);
            contextMock.Setup(x => x.TcpClient).Returns(tcpClientMock.Object);
            state.OnStateStart(contextMock.Object);
            dogMock.Raise(x => x.Errored += null, this, new Exception());
            contextMock.VerifySet(x => x.State = AdminConnectionState.Errored);
        }

        [Fact]
        public void CreateNewPlayerEntry_WhenReceiveMessageAboutHim()
        {
            var msg = new AdminServerClientInfoMessage()
            {
                ClientId = 11u,
                ClientName = "NewPlayer",
                Hostname = "127.0.0.1",
                JoinDate = new OttdDate(1, 1, 1),
                Language = 1,
                PlayingAs = 1
            };

            state.OnMessageReceived(msg, context);

            Assert.Equal("NewPlayer", context.Players[11u].Name);
        }

        [Fact]
        public void RemoveNewPlayerEntry_WhenReceiveMessageAboutQuiting()
        {
            var msg = new AdminServerClientInfoMessage()
            {
                ClientId = 11u,
                ClientName = "NewPlayer",
                Hostname = "127.0.0.1",
                JoinDate = new OttdDate(1, 1, 1),
                Language = 1,
                PlayingAs = 1
            };

            state.OnMessageReceived(msg, context);
            state.OnMessageReceived(new AdminServerClientQuitMessage(11u), context);

            Assert.False(context.Players.ContainsKey(11u));
        }


        [Fact]
        public void UpdatePlayerEntry_WhenReceiveUpdateMessage()
        {
            var msg = new AdminServerClientInfoMessage()
            {
                ClientId = 11u,
                ClientName = "NewPlayer",
                Hostname = "127.0.0.1",
                JoinDate = new OttdDate(1, 1, 1),
                Language = 1,
                PlayingAs = 1
            };

            state.OnMessageReceived(msg, context);

            var updateMessage = new AdminServerClientUpdateMessage(11u, "OldPlayer", 5);

            state.OnMessageReceived(updateMessage, context);

            Assert.Equal("OldPlayer", context.Players[11u].Name);
        }

        [Fact]
        public void NotDoAnythin_WhenTryingToConnectAgain()
        {
            // how should I test that xD?
            state.Connect(context);
            Assert.Equal(AdminConnectionState.Connected, context.State);
        }

        [Fact]
        public void ChangeToDisconnectingState_WhenCallingDisconnect()
        {
            state.Disconnect(context);
        }

        [Fact]
        public void SendMessageToClient_WhenReceivingMessage()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();
            state.SendMessage(msg, context);

            Assert.Empty(context.MessagesToSend);
            tcpClientMock.Verify(x => x.SendMessage(msg), Times.Once);
        }
    }
    }
