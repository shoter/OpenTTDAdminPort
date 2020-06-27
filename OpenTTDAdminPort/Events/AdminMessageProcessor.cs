using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Events
{
     internal class AdminMessageProcessor : IAdminMessageProcessor
    {
        public IAdminEvent? ProcessMessage(in IAdminMessage adminMessage, in IAdminPortClientContext context)
        {
            switch (adminMessage.MessageType)
            {
                case AdminMessageType.ADMIN_PACKET_SERVER_CHAT:
                    {
                        var msg = (AdminServerChatMessage)adminMessage;
                        if (msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
                            return null;
                        var player = context.Players[msg.ClientId];

                        return new AdminChatMessageEvent(player, msg.Message);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE:
                    {
                        var msg = (AdminServerConsoleMessage)adminMessage;

                        return new AdminConsoleEvent(msg.Origin, msg.Message);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_RCON:
                    {
                        var msg = (AdminServerRconMessage)adminMessage;

                        return new AdminRconEvent(msg.Result);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_PONG:
                    {
                        var msg = (AdminServerPongMessage)adminMessage;

                        return new AdminPongEvent(msg.Argument);
                    }
                default:
                    return null;
            }
        }
    }
}
