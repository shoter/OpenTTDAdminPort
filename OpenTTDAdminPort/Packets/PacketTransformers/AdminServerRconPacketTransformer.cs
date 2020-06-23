using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerRconPacketTransformer : IPacketTransformer<AdminServerRconMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_RCON;
        public IAdminMessage Transform(Packet packet) => new AdminServerRconMessage(packet.ReadU16(), packet.ReadString());
    }
}
