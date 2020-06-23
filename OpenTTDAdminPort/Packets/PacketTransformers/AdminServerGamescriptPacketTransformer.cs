using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerGamescriptPacketTransformer : IPacketTransformer<AdminServerGamescriptMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_GAMESCRIPT;

        public IAdminMessage Transform(Packet packet) => new AdminServerGamescriptMessage(packet.ReadString());
    }
}
