using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Packets
{
    internal interface IAdminPacketService
    {
        Packet CreatePacket(in IAdminMessage message);
        IAdminMessage ReadPacket(Packet packet);
    }
}
