using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyUpdatePacketTransformer : IPacketTransformer<AdminServerCompanyUpdateMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE;

        public IAdminMessage Transform(Packet packet)
        {
            var m = new AdminServerCompanyUpdateMessage();
            m.CompanyId = packet.ReadByte();
            m.CompanyName = packet.ReadString();
            m.ManagerName = packet.ReadString();
            m.Color = packet.ReadByte();
            m.HasPassword = packet.ReadBool();
            m.MonthsOfBankruptcy = packet.ReadByte();

            return m;
        }
    }
}
