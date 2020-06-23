using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerWelcomePacketTransformer : IPacketTransformer<AdminServerWelcomeMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_WELCOME;

        public IAdminMessage Transform(Packet packet)
        {
            return new AdminServerWelcomeMessage()
            {
                ServerName = packet.ReadString(),
                NetworkRevision = packet.ReadString(),
                IsDedicated = packet.ReadBool(),
                MapName = packet.ReadString(),
                MapSeed = packet.ReadU32(),
                Landscape = (Landscape)packet.ReadByte(),
                CurrentDate = new OttdDate(packet.ReadU32()),
                MapWidth = packet.ReadU16(),
                MapHeight = packet.ReadU16(),
            };
        }
    }
}
