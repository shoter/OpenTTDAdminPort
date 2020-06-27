using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortIdleStateShould : BaseStateShould
    {
        AdminPortIdleState state;

        public AdminPortIdleStateShould()
        {
            state = new AdminPortIdleState(adminPacketServiceMock.Object);
        }


        [Fact]
        public async Task StartTcpClientOnConnect_AndSendJoiningMessage()
        {
            await state.Connect(context);

            tcpClientMock.Verify(x => x.Start(context.ServerInfo.ServerIp, context.ServerInfo.ServerPort), Times.Once);
            tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg =>
            ((AdminJoinMessage)msg).Password == context.ServerInfo.Password &&
            ((AdminJoinMessage)msg).AdminName == context.ClientName &&
            ((AdminJoinMessage)msg).AdminVersion == context.ClientVersion
            )), Times.Once);
            Assert.Equal(AdminConnectionState.Connecting, context.State);
        }

        [Fact]
        public async Task NotTryToConnect_WhenTcpClientErrorsDuringStart()
        {
            tcpClientMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<int>())).Throws<Exception>();
            await Assert.ThrowsAsync<AdminPortException>(async () => await state.Connect(context));
            Assert.Equal(AdminConnectionState.ErroredOut, context.State);
        }
    }
}
