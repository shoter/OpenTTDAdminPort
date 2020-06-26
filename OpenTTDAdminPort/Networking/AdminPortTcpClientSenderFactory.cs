using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClientSenderFactory : IAdminPortTcpClientSenderFactory
    {
        private readonly IAdminPacketService packetService;

        public AdminPortTcpClientSenderFactory(IAdminPacketService packetService)
        {
            this.packetService = packetService;
        }
        public IAdminPortTcpClientSender Create(ITcpClient tcpClient)
        {
            return new AdminPortTcpClientSender(packetService, tcpClient.GetStream());
        }
    }
}
