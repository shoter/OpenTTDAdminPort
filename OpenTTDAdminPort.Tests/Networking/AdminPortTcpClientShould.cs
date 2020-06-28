using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientShould
    {
        IAdminPortTcpClient client;
        Mock<IAdminPortTcpClientReceiver> receiverMock = new Mock<IAdminPortTcpClientReceiver>();
        Mock<IAdminPortTcpClientSender> senderMock = new Mock<IAdminPortTcpClientSender>();
        Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();

        string ip = "127.0.0.1";
        int port = 69;

        public  AdminPortTcpClientShould()
        {
            client = new AdminPortTcpClient(senderMock.Object, receiverMock.Object, tcpClientMock.Object);
        }

        [Fact]
        public async Task ConnectToCorrectServer_OnStart()
        {
            await client.Start(ip, port);
            tcpClientMock.Verify(x => x.ConnectAsync(ip, port), Times.Once);
        }

        [Fact]
        public async Task StopReceiver_WhenSenderErrorsOut()
        {
            await client.Start(ip, port);
            senderMock.Raise(x => x.ErrorOcurred += null, this, new Exception());
            await Task.Delay(100);
            senderMock.Verify(x => x.Stop(), Times.AtLeastOnce);
            receiverMock.Verify(x => x.Stop(), Times.Once);
        }

        [Fact]
        public async Task StopSender_WhenReceiverErrorsOut()
        {
            await client.Start(ip, port);
            receiverMock.Raise(x => x.ErrorOcurred += null, this, new Exception());
            await Task.Delay(100);
            senderMock.Verify(x => x.Stop(), Times.Once);
            receiverMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SendMessageEvent_WhenMessageIsReceived()
        {
            await client.Start(ip, port);
            IAdminMessage msg = new AdminServerPongMessage(32u);
            IAdminMessage received = null;
            client.MessageReceived += (_, r) => received = r;
            receiverMock.Raise(x => x.MessageReceived += null, this, msg);
            Assert.Equal(msg, received);
        }

        [Fact]
        public async Task BeAbleToSendMessageToSender()
        {
            await client.Start(ip, port);
            IAdminMessage msg = new AdminPingMessage(33u);
            client.SendMessage(msg);
            senderMock.Verify(x => x.SendMessage(msg), Times.Once);
        }

        [Fact]
        public async Task ErrorOut_AfterSecondStart()
        {
            await client.Start(ip, port);
            await Assert.ThrowsAsync<AdminPortException>(async () => await client.Start(ip, port));
            Assert.Equal(WorkState.Errored, client.State);
            senderMock.Verify(x => x.Stop(), Times.Once);
            receiverMock.Verify(x => x.Stop(), Times.Once);
        }


        [Fact]
        public async Task BeAbleToStop()
        {
            await client.Start(ip, port);
            await client.Stop(Mock.Of<ITcpClient>());
            Assert.Equal(WorkState.Stopped, client.State);
            senderMock.Verify(x => x.Stop(), Times.Once);
            receiverMock.Verify(x => x.Stop(), Times.Once);
        }

        [Fact]
        public async Task StartAfterStop()
        {
            var tcpClientNewMock = new Mock<ITcpClient>();
            await client.Start(ip, port);
            await client.Stop(tcpClientNewMock.Object);
            await client.Start(ip, port);
            Assert.Equal(WorkState.Working, client.State);
            tcpClientNewMock.Verify(x => x.ConnectAsync(ip, port), Times.Once);
        }

        [Fact]
        public async Task SttopAndStartSenderReceiver_AfterRestart()
        {
            var newTcpClientMock = new Mock<ITcpClient>();
            Stream someStream = new MemoryStream();
            newTcpClientMock.Setup(x => x.GetStream()).Returns(someStream);
            await client.Start(ip, port);
            await client.Restart(newTcpClientMock.Object);

            senderMock.Verify(x => x.Stop(), Times.Once);
            receiverMock.Verify(x => x.Stop(), Times.Once);

            senderMock.Verify(x => x.Start(someStream), Times.Once);
            receiverMock.Verify(x => x.Start(someStream), Times.Once);
        }
    }
}
