using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortIdleState : BaseAdminPortClientState
    {
        public override async Task Connect(IAdminPortClientContext context)
        {
            try
            {
                await context.TcpClient.Start(context.ServerInfo.ServerIp, context.ServerInfo.ServerPort);
                context.TcpClient.SendMessage(new AdminJoinMessage(context.ServerInfo.Password, context.ClientName, context.ClientVersion));
                context.State = AdminConnectionState.Connecting;

               
            }
            catch (Exception e)
            {
                context.State = AdminConnectionState.ErroredOut;
                throw new AdminPortException("Could not join server", e);
            }
        }

        public override Task Disconnect(IAdminPortClientContext context)
        {
            return Task.CompletedTask;
        }
    }
}
