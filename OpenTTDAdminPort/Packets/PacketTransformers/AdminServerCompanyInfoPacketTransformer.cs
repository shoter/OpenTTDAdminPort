using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyInfoPacketTransformer : IPacketTransformer<AdminServerCompanyInfoMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_INFO;
        public IAdminMessage Transform(Packet packet)
        {
            var m = new AdminServerCompanyInfoMessage();
            m.CompanyId = packet.ReadByte();
            m.CompanyName = packet.ReadString();
            m.ManagerName = packet.ReadString();
            m.Color = packet.ReadByte();
            m.HasPassword = packet.ReadBool();
            m.CreationDate = new OttdDate(packet.ReadU32());
            m.IsAi = packet.ReadBool();
            m.MonthsOfBankruptcy = packet.ReadByte();
            for (int i = 0; i < m.ShareOwnersIds.Length; ++i) m.ShareOwnersIds[i] = packet.ReadByte();

            return m;
        }
    }
}
