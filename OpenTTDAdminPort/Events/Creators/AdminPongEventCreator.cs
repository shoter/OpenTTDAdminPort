using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminPongEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;

        public IAdminEvent? Create(in IAdminMessage message, in IAdminPortClientContext context)
        {
            var msg = (AdminServerPongMessage)message;

            return new AdminPongEvent(msg.Argument);
        }
    }
}
