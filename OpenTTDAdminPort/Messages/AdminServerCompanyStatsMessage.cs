using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Messages
{
    public class AdminServerCompanyStatsMessage : IAdminMessage
    {
        public struct AdminServerCompanyStats
        {
            public ushort[] VehicleCount { get; }

            public ushort[] StationCount { get; }

            public AdminServerCompanyStats(ushort[] vehCount, ushort[] stationCount)
            {
                this.VehicleCount = vehCount;
                this.StationCount = stationCount;
            }
        }

        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_STATS;

        public Dictionary<byte, AdminServerCompanyStats> CompanyStats { get; }

        public AdminServerCompanyStatsMessage(Dictionary<byte, AdminServerCompanyStats> companyStats)
        {
            this.CompanyStats = companyStats;
        }
    }
}
