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
    public class AdminPortTcpClientSenderFactoryShould
    {
        IAdminPortTcpClientSenderFactory factory;

        public AdminPortTcpClientSenderFactoryShould()
        {
            factory = new AdminPortTcpClientSenderFactory(Mock.Of<IAdminPacketService>());
        }

        [Fact]
        public void CreateObject()
        {
            IAdminPortTcpClientSender sender = factory.Create(Mock.Of<ITcpClient>());
            Assert.NotNull(sender);
        }
    }
}
