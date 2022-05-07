using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientUpdateEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData context)
        {
            var msg = (AdminServerClientUpdateMessage)message;
            var player = context.Players[msg.ClientId];

            return new AdminClientUpdateEvent(player);
        }
    }
}
