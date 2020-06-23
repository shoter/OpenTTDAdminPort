using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerPongPacketTransformer : IPacketTransformer<AdminServerPongMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;
        public IAdminMessage Transform(Packet packet) => new AdminServerConsoleMessage(packet.ReadString(), packet.ReadString());
    }
}
