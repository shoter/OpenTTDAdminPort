using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerCompanyInfoMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_INFO;

        public byte CompanyId { get; internal set; }

        public string CompanyName { get; internal set; } = default!;

        public string ManagerName { get; internal set; } = default!;

        public byte Color { get; internal set; }

        public bool HasPassword { get; internal set; }

        /// <summary>
        /// Defines when company was created.
        /// </summary>
        public OttdDate CreationDate { get; internal set; } = default!;

        public bool IsAi { get; internal set; }

        /// <summary>
        /// I do not know what this value represents. I will not need it right now but if there is a nice soul that would like to leave better comment - please do! :)
        /// </summary>
        public byte MonthsOfBankruptcy { get; internal set; }
    }
}
