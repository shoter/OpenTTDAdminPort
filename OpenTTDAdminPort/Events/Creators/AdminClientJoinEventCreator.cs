﻿using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientJoinEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData context)
        {
            var msg = (AdminServerClientJoinMessage)message;
            var player = context.Players[msg.ClientId];

            return new AdminClientJoinEvent(player);
        }
    }
}
