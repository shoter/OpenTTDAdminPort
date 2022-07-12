using System.Collections.Generic;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCmdNamesPacketTransformer : IPacketTransformer<AdminServerCmdNamesMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES;

        public IAdminMessage Transform(Packet packet)
        {
            Dictionary<ushort, string> commands = new Dictionary<ushort, string>();
            while (packet.ReadBool())
            {
                commands.Add(packet.ReadU16(), packet.ReadString());
            }

            return new AdminServerCmdNamesMessage(commands);
        }
    }
}
