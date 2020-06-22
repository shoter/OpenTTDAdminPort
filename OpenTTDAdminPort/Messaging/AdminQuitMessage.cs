using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTTDAdminPort.Messaging
{
    class AdminQuitMessage : IAdminMessage 
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_QUIT;
    }
}
