using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerClientUpdatePacketTransformer : IPacketTransformer<AdminServerClientUpdateMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE;

        public IAdminMessage Transform(Packet packet)
        {
            uint clientId = packet.ReadU32();
            string clientName = packet.ReadString();
            byte playingAs = packet.ReadByte();
            return new AdminServerClientUpdateMessage(clientId, clientName, playingAs);
        }
    }
}
