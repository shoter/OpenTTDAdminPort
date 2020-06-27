using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortConnectedState : BaseAdminPortClientState
    {
        public override void OnStateStart(IAdminPortClientContext context)
        {
            base.OnStateStart(context);
            //we need info about clients.
            context.TcpClient.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CHAT, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
            context.TcpClient.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CONSOLE, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
            context.TcpClient.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
            context.TcpClient.SendMessage(new AdminPollMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, uint.MaxValue));

            // we need to send all old messages.
            while (context.MessagesToSend.TryDequeue(out IAdminMessage msg))
            {
                context.TcpClient.SendMessage(msg);
            }
        }

        override 

        public void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context)
        {

            switch(message.MessageType)
            {
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO:
                    {
                        var msg = (AdminServerClientInfoMessage)message;
                        var player = new Player(msg.ClientId, msg.ClientName);
                        context.Players.AddOrUpdate(msg.ClientId, player, (_, __) => player);

                        break;
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE:
                    {
                        var msg = (AdminServerClientUpdateMessage)message;
                        var player = context.Players[msg.ClientId];
                        player.Name = msg.ClientName;
                        break;
                    }
                default:
                    {
                        break;

                    }
            }
        }

        public override Task Connect(IAdminPortClientContext context)
        {
            return Task.CompletedTask;
        }

        public override Task Disconnect(IAdminPortClientContext context)
        {
            context.State = AdminConnectionState.Disconnecting;
            return Task.CompletedTask;
        }
    }
}
