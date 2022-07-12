using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerCompanyEconomyMessage : IAdminMessage
    {
        public class QuarterData
        {
            public ulong CompanyValue { get; }

            public ushort PerformanceHistory { get; }

            public ushort DeliveredCargo { get; }

            public QuarterData(ulong companyValue, ushort perfHistory, ushort deliveredCargo)
            {
                CompanyValue = companyValue;
                PerformanceHistory = perfHistory;
                DeliveredCargo = deliveredCargo;
            }
        }

        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_ECONOMY;

        public byte CompanyId { get; internal set; }

        public ulong Money { get; internal set; }

        public ulong CurrentLoan { get; internal set; }

        public ulong Income { get; internal set; }

        public ushort DeliveredCargo { get; internal set; }

        public QuarterData[] Quarters { get; internal set; } = default!;
    }
}
