using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerChatPacketTransformer : IPacketTransformer<AdminServerChatMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CHAT;

        public IAdminMessage Transform(Packet packet)
        {
            var networkAction = (NetworkAction)packet.ReadByte();
            var chatDestination = (ChatDestination)packet.ReadByte();
            var clientId = packet.ReadU32();
            var message = packet.ReadString();
            var data = packet.ReadI64();

            return new AdminServerChatMessage(
                networkAction,
                chatDestination,
                clientId,
                message,
                data);
        }
    }
}
