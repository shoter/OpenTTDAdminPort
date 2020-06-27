using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortDisconnectedState : BaseAdminPortClientState
    {
        public override void OnStateStart(IAdminPortClientContext context)
        {
            base.OnStateStart(context);
            context.TcpClient.SendMessage(new AdminQuitMessage());
            context.TcpClient.Restart(new MyTcpClient());
            context.State = AdminConnectionState.Idle;
        }
        public override Task Connect(IAdminPortClientContext context)
        {
            throw new AdminPortException("Cannot connect while disconnecting");
        }

        public override Task Disconnect(IAdminPortClientContext context)
        {
            return Task.CompletedTask;
        }
    }
}
