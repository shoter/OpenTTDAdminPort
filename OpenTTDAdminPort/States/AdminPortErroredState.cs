using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortErroredState : BaseAdminPortClientState
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
            try
            {
                context.TcpClient.SendMessage(new AdminQuitMessage());
            }
            catch (Exception) { }
            context.TcpClient.Restart();
            context.TcpClient.SendMessage(new AdminJoinMessage(context.ServerInfo.Password, context.ClientName, context.ClientVersion));
            context.MessagesToSend.Clear();
            context.State = AdminConnectionState.Connecting;
        }
    }
}
