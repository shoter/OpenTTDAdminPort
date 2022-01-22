using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortErroredStateShould : BaseStateShould
    {
        AdminPortErroredState state = new AdminPortErroredState();

        public AdminPortErroredStateShould()
        {
            context.State = AdminConnectionState.Errored;
        }

        [Fact]
        public void RecoverNormalState_OnStateStart()
        {
            state.OnStateStart(context);

            Assert.Equal(AdminConnectionState.Connecting, context.State);
            tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg => msg is AdminJoinMessage)), Times.Once);
            tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg => msg is AdminQuitMessage)), Times.Once);
            tcpClientMock.Verify(x => x.Restart(), Times.Once);
        }

        [Fact]
        public void ClearMessagesToSend_OnStateStart()
        {
            context.MessagesToSend.Enqueue(Mock.Of<IAdminMessage>());
            state.OnStateStart(context);
            Assert.Empty(context.MessagesToSend);
        }

        [Fact]
        public void NotErrorOut_WhenSendingMessageWillFail()
        {
            tcpClientMock.Setup(x => x.SendMessage(It.IsAny<IAdminMessage>())).Throws<Exception>();
            tcpClientMock.Setup(x => x.Restart())
                .Callback(() => tcpClientMock.Setup(x => x.SendMessage(It.IsAny<IAdminMessage>())));
            state.OnStateStart(context);
            Assert.Equal(AdminConnectionState.Connecting, context.State);
        }

        [Fact]
        public void PutMessagesIntoQueue_WhenUserWantsToSendMessage()
        {
            IAdminMessage msg = Mock.Of<IAdminMessage>();
            state.SendMessage(msg, context);

            Assert.Contains(msg, context.MessagesToSend);
        }

        [Fact]
        public async Task GoIntoDisconnectState_OnDisconnect()
        {
            await state.Disconnect(context);
            Assert.Equal(AdminConnectionState.Disconnecting, context.State);
        }

        [Fact]
        public async Task DoNothing_OnConnect()
        {
            await state.Connect(context);
            Assert.Equal(AdminConnectionState.Errored, context.State);
        }


    }
}
