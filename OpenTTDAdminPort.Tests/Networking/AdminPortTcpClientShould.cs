using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
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
            Mock<IAdminPortTcpClientReceiverFactory> receiverFactory = new Mock<IAdminPortTcpClientReceiverFactory>();
            receiverFactory.Setup(x => x.Create(It.IsAny<ITcpClient>())).Returns(receiverMock.Object);
            Mock<IAdminPortTcpClientSenderFactory> senderFactory = new Mock<IAdminPortTcpClientSenderFactory>();
            senderFactory.Setup(x => x.Create(It.IsAny<ITcpClient>())).Returns(senderMock.Object);

            client = new AdminPortTcpClient(senderFactory.Object, receiverFactory.Object, tcpClientMock.Object, ip, port);
        }

        [Fact]
        public async Task ConnectToCorrectServer_OnStart()
        {
            await client.Start();
            tcpClientMock.Verify(x => x.ConnectAsync(ip, port), Times.Once);
        }

        [Fact]
        public async Task StartSenderReceiver_OnStart()
        {
            await client.Start();
            senderMock.Verify(x => x.Start(), Times.Once);
            receiverMock.Verify(x => x.Start(), Times.Once);
        }

        [Fact]
        public async Task StopReceiver_WhenSenderErrorsOut()
        {
            await client.Start();
            senderMock.Raise(x => x.ErrorOcurred += null, this, new Exception());
            await Task.Delay(100);
            senderMock.Verify(x => x.Start(), Times.AtLeastOnce);
            receiverMock.Verify(x => x.Start(), Times.Once);
        }

        [Fact]
        public async Task StopSender_WhenReceiverErrorsOut()
        {
            await client.Start();
            receiverMock.Raise(x => x.ErrorOcurred += null, this, new Exception());
            await Task.Delay(100);
            senderMock.Verify(x => x.Start(), Times.Once);
            receiverMock.Verify(x => x.Start(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task SendMessageEvent_WhenMessageIsReceived()
        {
            await client.Start();
            IAdminMessage msg = new AdminServerPongMessage(32u);
            IAdminMessage received = null;
            client.MessageReceived += (_, r) => received = r;
            receiverMock.Raise(x => x.MessageReceived += null, this, msg);
            Assert.Equal(msg, received);
        }

        [Fact]
        public async Task BeAbleToSendMessageToSender()
        {
            await client.Start();
            IAdminMessage msg = new AdminPingMessage(33u);
            client.SendMessage(msg);
            senderMock.Verify(x => x.SendMessage(msg), Times.Once);
        }

    }
}
