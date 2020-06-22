using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Messaging
{
    public class AdminGamescriptMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_GAMESCRIPT;

        public string Json { get; }

        public AdminGamescriptMessage(string json)
        {
            this.Json = json;
        }
    }
}
