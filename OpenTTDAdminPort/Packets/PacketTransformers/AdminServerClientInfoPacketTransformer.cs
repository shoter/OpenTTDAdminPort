using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerClientInfoPacketTransformer : IPacketTransformer<AdminServerClientInfoMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;
        public IAdminMessage Transform(Packet packet) => new AdminServerClientInfoMessage()
        {
            ClientId = packet.ReadU32(),
            Hostname = packet.ReadString(),
            ClientName = packet.ReadString(),
            Language = packet.ReadByte(),
            JoinDate = new OttdDate(packet.ReadU32()),
            PlayingAs = packet.ReadByte(),
        };
    }
}
