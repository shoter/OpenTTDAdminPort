﻿using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    public class AdminPortConnectedState : BaseAdminPortClientState
    {
        public override void OnStateStart(AdminPortClientContext context)
        {
            base.OnStateStart(context);
            while(context.MessagesToSend.TryDequeue(out IAdminMessage msg))
            {
                context.TcpClient.SendMessage(msg);
            }
        }

        protected override void MessageReceived(AdminPortClientContext context, IAdminMessage message)
        {
            base.MessageReceived(context, message);

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

        public override Task Connect(AdminPortClientContext context)
        {
            return Task.CompletedTask;
        }

        public override Task Disconnect(AdminPortClientContext context)
        {
            context.State = AdminConnectionState.Disconnecting;
            return Task.CompletedTask;
        }
    }
}
