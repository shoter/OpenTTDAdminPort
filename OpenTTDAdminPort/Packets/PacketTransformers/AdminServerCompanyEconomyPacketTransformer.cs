using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyEconomyPacketTransformer : IPacketTransformer<AdminServerCompanyEconomyMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_ECONOMY;

        public IAdminMessage Transform(Packet packet)
        {
            var m = new AdminServerCompanyEconomyMessage();
            m.CompanyId = packet.ReadByte();
            m.Money = packet.ReadU64();
            m.CurrentLoan = packet.ReadU64();
            m.Income = packet.ReadU64();
            m.DeliveredCargo = packet.ReadU16();
            m.Quarters = new AdminServerCompanyEconomyMessage.QuarterData[2];

            for (int i = 0; i < 2; ++i)
            {
                m.Quarters[i] = new AdminServerCompanyEconomyMessage.QuarterData(
                    companyValue: packet.ReadU64(),
                    perfHistory: packet.ReadU16(),
                    deliveredCargo: packet.ReadU16());
            }

            return m;
        }
    }
}
