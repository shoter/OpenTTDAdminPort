using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerClientErrorPacketTransformer : IPacketTransformer<AdminServerClientErrorMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR;

        public IAdminMessage Transform(Packet packet) => new AdminServerClientErrorMessage(packet.ReadU32(), packet.ReadByte());
    }
}
