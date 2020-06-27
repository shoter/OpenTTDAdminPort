using Moq;
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
    public class AdminPortIdleStateShould
    {
        AdminPortClientContextFake fix = new AdminPortClientContextFake();
        AdminPortIdleState state;
        Mock<IAdminPacketService> adminPacketServiceMock = new Mock<IAdminPacketService>();

        public AdminPortIdleStateShould()
        {
            state = new AdminPortIdleState(adminPacketServiceMock.Object);
        }


        [Fact]
        public async Task StartTcpClientOnConnect()
        {
            var context = new AdminPortClientContextFake();
            await state.Connect(context);

            context.FakeTcpClient.Verify(x => x.Start(context.ServerInfo.ServerIp, context.ServerInfo.ServerPort), Times.Once);
            Assert.Equal(AdminConnectionState.Connecting, context.State);
        }

        [Fact]
        public async Task NotTryToConnect_WhenTcpClientErrorsDuringStart()
        {
            var context = new AdminPortClientContextFake();
            context.FakeTcpClient.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<int>())).Throws<Exception>();
            await Assert.ThrowsAsync<AdminPortException>(async () => await state.Connect(context));
            Assert.Equal(AdminConnectionState.ErroredOut, context.State);
        }
    }
}
