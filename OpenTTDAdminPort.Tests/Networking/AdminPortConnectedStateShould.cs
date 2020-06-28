using Moq;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
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
