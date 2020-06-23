using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerDatePacketTransformer : IPacketTransformer<AdminServerDateMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_DATE;
        public IAdminMessage Transform(Packet packet) => new AdminServerDateMessage(packet.ReadU32());
    }
}
