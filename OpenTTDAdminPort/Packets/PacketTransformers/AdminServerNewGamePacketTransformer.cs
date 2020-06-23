using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerNewGamePacketTransformer : IPacketTransformer<AdminServerNewGameMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_NEWGAME;

        public IAdminMessage Transform(Packet packet) => new AdminServerNewGameMessage();
    }
}
