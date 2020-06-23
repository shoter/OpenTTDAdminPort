using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;
using System.Collections.Generic;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCmdNamesPacketTransformer : IPacketTransformer<AdminServerCmdNamesMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES;

        public IAdminMessage Transform(Packet packet)
        {
            Dictionary<ushort, string> commands = new Dictionary<ushort, string>();
            while(packet.ReadBool())
            {
                commands.Add(packet.ReadU16(), packet.ReadString());
            }

            return new AdminServerCmdNamesMessage(commands);
        }
    }
}
