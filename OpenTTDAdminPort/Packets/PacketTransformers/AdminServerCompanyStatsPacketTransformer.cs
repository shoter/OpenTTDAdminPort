using System.Collections.Generic;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerCompanyStatsPacketTransformer : IPacketTransformer<AdminServerCompanyStatsMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_STATS;

        public IAdminMessage Transform(Packet packet)
        {
            var companyStats = new Dictionary<byte, AdminServerCompanyStatsMessage.AdminServerCompanyStats>();

            // why the fuck they are not sending number of companies - this stinks of being unreliable.
            while (packet.Position < packet.Size)
            {
                ushort[] vehCount = new ushort[(int)NetworkVehicleType.NETWORK_VEH_END];
                ushort[] stationCount = new ushort[(int)NetworkVehicleType.NETWORK_VEH_END];

                byte index = packet.ReadByte();

                for (int i = 0; i < (int)NetworkVehicleType.NETWORK_VEH_END; ++i)
                {
                    vehCount[i] = packet.ReadU16();
                }

                for (int i = 0; i < (int)NetworkVehicleType.NETWORK_VEH_END; ++i)
                {
                    stationCount[i] = packet.ReadU16();
                }

                companyStats.Add(index, new AdminServerCompanyStatsMessage.AdminServerCompanyStats(vehCount, stationCount));
            }

            return new AdminServerCompanyStatsMessage(companyStats);
        }
    }
}
