using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyRemovePacketTransformer : IPacketTransformer<AdminServerCompanyRemoveMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE;
        public IAdminMessage Transform(Packet packet) => new AdminServerCompanyRemoveMessage(packet.ReadByte(), packet.ReadByte());
    }
}
