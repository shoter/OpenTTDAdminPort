using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerDateMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_DATE;

        public OttdDate Date { get; }

        public AdminServerDateMessage(uint date)
        {
            this.Date = new OttdDate(date);
        }
    }
}
