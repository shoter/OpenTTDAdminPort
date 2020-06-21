using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Messaging
{
    internal class AdminServerClientQuitMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT;

        public uint ClientId { get; }

        public AdminServerClientQuitMessage(uint clientId) => this.ClientId = clientId;
    }
}
