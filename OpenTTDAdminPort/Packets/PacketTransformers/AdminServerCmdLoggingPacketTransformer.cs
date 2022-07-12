using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCmdLoggingPacketTransformer : IPacketTransformer<AdminServerCmdLoggingMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_LOGGING;

        public IAdminMessage Transform(Packet packet) => new AdminServerCmdLoggingMessage()
        {
            ClientId = packet.ReadU32(),
            CompanyId = packet.ReadByte(),
            Cmd = packet.ReadU16(),
            P1 = packet.ReadU32(),
            P2 = packet.ReadU32(),
            Tile = packet.ReadU32(),
            Text = packet.ReadString(),
            Frame = packet.ReadU32(),
        };
    }
}
