using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerConsolePacketTransformer : IPacketTransformer<AdminServerConsoleMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE;
        public IAdminMessage Transform(Packet packet) => new AdminServerPongMessage(packet.ReadU32());
    }
}
