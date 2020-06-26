using Moq;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientReceiverFactoryShould
    {
        IAdminPortTcpClientReceiverFactory factory;

        public AdminPortTcpClientReceiverFactoryShould()
        {
            factory = new AdminPortTcpClientReceiverFactory(Mock.Of<IAdminPacketService>());
        }

        [Fact]
        public void CreateObject()
        {
            IAdminPortTcpClientReceiver receiver = factory.Create(Mock.Of<ITcpClient>());
            Assert.NotNull(receiver);
        }
    }
}
