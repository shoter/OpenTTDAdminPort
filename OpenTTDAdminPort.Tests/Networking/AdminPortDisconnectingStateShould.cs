using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortDisconnectingStateShould : BaseStateShould
    {
        AdminPortDisconnectingState state = new AdminPortDisconnectingState();

        public AdminPortDisconnectingStateShould()
        {
            context.State = AdminConnectionState.Disconnecting;

        }

        [Fact]
        public void DoProperDisconnect_OnStateStart()
        {
            state.OnStateStart(context);

            tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(m => m is AdminQuitMessage)), Times.Once);
            tcpClientMock.Verify(x => x.Stop(It.IsAny<ITcpClient>()), Times.Once);
            Assert.Equal(AdminConnectionState.Idle, context.State);
        }

        [Fact]
        public async Task DoNothing_WhenDisconnectCalled()
        {
            await state.Disconnect(context);
            Assert.Equal(AdminConnectionState.Disconnecting, context.State);
        }

        [Fact]
        public void ShouldNotSendMessage_OnSendMessage()
        {
            var msg = new AdminPingMessage();
            state.SendMessage(msg, context);

            tcpClientMock.Verify(x => x.SendMessage(msg), Times.Never);
            Assert.Empty(context.MessagesToSend);
        }

        [Fact]
        public async Task ThrowException_WhenTryingToCallConnect()
        {
            await Assert.ThrowsAsync<AdminPortException>(async() => await state.Connect(context));
        }



    }
}
