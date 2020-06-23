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
            var m = new AdminServerChatMessage();
            m.NetworkAction = (NetworkAction)packet.ReadByte();
            m.ChatDestination = (ChatDestination)packet.ReadByte();
            m.ClientId = packet.ReadU32();
            m.Message = packet.ReadString();
            m.Data = packet.ReadI64();

            return m;
        }
    }
}
