using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerShutdownPacketTransformer : IPacketTransformer<AdminServerShutdownMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_SHUTDOWN;

        public IAdminMessage Transform(Packet packet) => new AdminServerShutdownMessage();
    }
}
