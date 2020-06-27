using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal abstract class BaseAdminPortClientState : IAdminPortClientState
    {
        public abstract Task Connect(IAdminPortClientContext context);

        public abstract Task Disconnect(IAdminPortClientContext context);

        virtual public void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context)
        {
        }

        virtual public void SendMessage(IAdminMessage message, IAdminPortClientContext context)
        {
            context.MessagesToSend.Enqueue(message);
        }

        virtual public void OnStateEnd(IAdminPortClientContext context)
        {
        }

        virtual public void OnStateStart(IAdminPortClientContext context)
        {
        }
    }
}
