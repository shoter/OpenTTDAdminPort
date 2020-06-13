using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    internal interface IAdminPacketService
    {
        Packet CreatePacket(IAdminMessage message);
        IAdminMessage ReadPacket(Packet packet);
    }
}
