using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Messaging
{
    public class AdminRconMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_RCON;

        public string Command { get; }

        public AdminRconMessage(string command)
        {
            this.Command = command;
        }

    }
}
