using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortDisconnectingState : BaseAdminPortClientState
    {
        public override void OnStateStart(IAdminPortClientContext context)
        {
            base.OnStateStart(context);
            context.TcpClient.SendMessage(new AdminQuitMessage());
            context.TcpClient.Stop().Wait();
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

        public override void SendMessage(IAdminMessage message, IAdminPortClientContext context)
        {
            // we are ignoring messages. They will probably do not make sense anyway if server will be started again.
        }
    }
}
