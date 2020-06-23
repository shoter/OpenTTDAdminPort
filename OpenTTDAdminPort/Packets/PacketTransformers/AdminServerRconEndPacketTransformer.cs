using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerRconEndPacketTransformer : IPacketTransformer<AdminServerRconEndMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_RCON_END;

        public IAdminMessage Transform(Packet packet) => new AdminServerRconEndMessage(packet.ReadString());
    }
}
