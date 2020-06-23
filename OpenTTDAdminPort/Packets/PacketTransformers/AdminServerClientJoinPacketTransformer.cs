using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerClientJoinPacketTransformer : IPacketTransformer<AdminServerClientJoinMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN;
        public IAdminMessage Transform(Packet packet) => new AdminServerClientJoinMessage(packet.ReadU32());;
    }
}
