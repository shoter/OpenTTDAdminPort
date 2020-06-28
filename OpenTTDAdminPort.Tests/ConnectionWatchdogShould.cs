using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Tests.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests
{
    public class ConnectionWatchdogShould
    {
        Mock<AdminPortTcpClientFake> tcpClientMock = new Mock<AdminPortTcpClientFake>();

        public ConnectionWatchdogShould()
        {
            tcpClientMock.CallBase = true;
        }

        [Fact]
        public async Task ErrorOut_IfServerDoesNotRespondInTime()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(0.2));
            Exception e = null;
            dog.Errored += (_, ex) => e = ex;
            dog.Start(tcpClientMock.Object);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.NotNull(e);
            Assert.False(dog.Enabled);
        }

        [Fact]
        public async Task NotErrorOut_WhenFakeClientWillRespondWithPongs()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(0.1));
            Exception e = null;
            dog.Errored += (_, ex) => e = ex;
            tcpClientMock.Setup(x => x.SendMessage(It.IsAny<IAdminMessage>()))
                .Callback((IAdminMessage msg) => {
                    var pingMsg = (AdminPingMessage)msg;
                    var pongMsg = new AdminServerPongMessage(pingMsg.Argument);
                    tcpClientMock.Object.SimulateMessageReceived(pongMsg);
                });
            dog.Start(tcpClientMock.Object);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Null(e);
            Assert.True(dog.Enabled);
        }

        [Fact]
        public async Task ErrorOut_WhenPongMsgArgumentIsNotMatchingPing()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(0.1));
            Exception e = null;
            dog.Errored += (_, ex) => e = ex;
            tcpClientMock.Setup(x => x.SendMessage(It.IsAny<IAdminMessage>()))
                .Callback((IAdminMessage msg) => {
                    var pingMsg = (AdminPingMessage)msg;
                    var pongMsg = new AdminServerPongMessage(pingMsg.Argument + 1);
                    tcpClientMock.Object.SimulateMessageReceived(pongMsg);
                });
            dog.Start(tcpClientMock.Object);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.NotNull(e);
            Assert.False(dog.Enabled);
        }

        [Fact]
        public async Task NotErrorOut_AfterWatchdogIsStopped()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(0.1));
            Exception e = null;
            dog.Errored += (_, ex) => e = ex;
            dog.Start(tcpClientMock.Object);
            dog.Stop();
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Null(e);
            Assert.False(dog.Enabled);
        }

        [Fact]
        public async Task BeAbleToStartAgain_AndWorkCorrect()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(0.1));
            Exception e = null;
            dog.Errored += (_, ex) => e = ex;
            dog.Start(tcpClientMock.Object);
            dog.Stop();
            dog.Start(tcpClientMock.Object);
            tcpClientMock.Setup(x => x.SendMessage(It.IsAny<IAdminMessage>()))
               .Callback((IAdminMessage msg) => {
                   var pingMsg = (AdminPingMessage)msg;
                   var pongMsg = new AdminServerPongMessage(pingMsg.Argument);
                   tcpClientMock.Object.SimulateMessageReceived(pongMsg);
               });
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Null(e);
            Assert.True(dog.Enabled);
        }

        [Fact]
        public void ThrowException_WhenTryingToStartItWithDifferentClientWhileItIsWorking()
        {
            var dog = new ConnectionWatchdog(TimeSpan.FromSeconds(5000));

            dog.Start(tcpClientMock.Object);
            Assert.Throws<AdminPortException>(() => dog.Start(Mock.Of<IAdminPortTcpClient>()));
        }
    }
}
