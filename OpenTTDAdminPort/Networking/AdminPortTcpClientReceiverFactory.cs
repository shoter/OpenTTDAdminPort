using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClientReceiverFactory : IAdminPortTcpClientReceiverFactory
    {
        private readonly IAdminPacketService packetService;
        public AdminPortTcpClientReceiverFactory(IAdminPacketService packetService)
        {
            this.packetService = packetService;
        }

        public IAdminPortTcpClientReceiver Create(ITcpClient tcpClient)
        {
            return new AdminPortTcpClientReceiver(packetService, tcpClient.GetStream());
        }
    }
}
