using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerClientQuitPacketTransformer : IPacketTransformer<AdminServerClientQuitMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT;
        public IAdminMessage Transform(Packet packet) => new AdminServerClientQuitMessage(packet.ReadU32());
    }
}
