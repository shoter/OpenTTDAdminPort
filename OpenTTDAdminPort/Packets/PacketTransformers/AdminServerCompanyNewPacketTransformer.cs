using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyNewPacketTransformer : IPacketTransformer<AdminServerCompanyNewMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_NEW;

        public IAdminMessage Transform(Packet packet) => new AdminServerCompanyNewMessage(packet.ReadByte());
    }
}
