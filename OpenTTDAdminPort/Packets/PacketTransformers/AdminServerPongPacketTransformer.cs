using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerPongPacketTransformer : IPacketTransformer<AdminServerPongMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;
        public IAdminMessage Transform(Packet packet) => new AdminServerPongMessage(packet.ReadU32());
    }
}
