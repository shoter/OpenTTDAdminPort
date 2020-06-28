using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortErroredOutStateShould : BaseStateShould
    {
        AdminPortErroredOutState state = new AdminPortErroredOutState();

        public AdminPortErroredOutStateShould()
        {
            context.State = AdminConnectionState.ErroredOut;
        }

        [Fact]
        public void ThrowException_ForApiUsedByUser()
        {
            Assert.Throws<AdminPortException>(() => state.SendMessage(Mock.Of<IAdminMessage>(), context));
            Assert.ThrowsAsync<AdminPortException>(async () => await state.Connect(context));
            Assert.ThrowsAsync<AdminPortException>(async () => await state.Disconnect(context));
        }

        [Fact]
        public void ClearMessageQueue_WhenStateStarts()
        {
            context.MessagesToSend.Enqueue(Mock.Of<IAdminMessage>());
            state.OnStateStart(context);
            Assert.Empty(context.MessagesToSend);
        }

        [Fact]
        public void DoNothing_OnMethodsExecutedOnStateChangeOrReceivedMessage()
        {
            state.OnStateStart(context);
            state.OnMessageReceived(Mock.Of<IAdminMessage>(), context);
            state.OnStateEnd(context);

            Assert.Empty(context.MessagesToSend);
            tcpClientMock.Verify(x => x.SendMessage(It.IsAny<IAdminMessage>()), Times.Never);
            Assert.Equal(AdminConnectionState.ErroredOut, context.State);
        }
    }
}
