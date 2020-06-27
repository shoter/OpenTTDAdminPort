using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortErroredState :BaseAdminPortClientState 
    {
        public override Task Connect(IAdminPortClientContext context)
        {
            return Task.CompletedTask;
        }

        public override Task Disconnect(IAdminPortClientContext context)
        {
            context.State = AdminConnectionState.Disconnecting;
            return Task.CompletedTask;
        }

        public override void OnStateStart(IAdminPortClientContext context)
        {
            base.OnStateStart(context);
            context.TcpClient.SendMessage(new AdminQuitMessage());
            context.TcpClient.Restart(new MyTcpClient());
            context.TcpClient.SendMessage(new AdminJoinMessage(context.ServerInfo.Password, context.ClientName, context.ClientVersion));
            context.State = AdminConnectionState.Connecting;
        }

    }
}
