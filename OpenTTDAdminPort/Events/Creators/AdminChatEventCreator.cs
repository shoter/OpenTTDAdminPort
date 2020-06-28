using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminChatEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CHAT;

        public IAdminEvent? Create(in IAdminMessage message, in IAdminPortClientContext context)
        {
            var msg = (AdminServerChatMessage)message;
            var player = context.Players[msg.ClientId];

            return new AdminChatMessageEvent(player, msg.ChatDestination, msg.NetworkAction, msg.Message);
        }
    }
}
