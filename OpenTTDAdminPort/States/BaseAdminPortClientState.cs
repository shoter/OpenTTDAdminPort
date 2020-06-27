using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    public abstract class BaseAdminPortClientState : IAdminPortClientState
    {
        public abstract Task Connect(AdminPortClientContext context);

        public abstract Task Disconnect(AdminPortClientContext context);

        virtual public void OnMessageReceived(IAdminMessage message, AdminPortClientContext context)
        {
        }

        virtual public void OnStateEnd(AdminPortClientContext context)
        {
            context.MessageReceived += Context_MessageReceived;
        }

        virtual public void OnStateStart(AdminPortClientContext context)
        {
            context.MessageReceived -= Context_MessageReceived;
        }

        private void Context_Errored(object sender, Exception e)
        {
            AdminPortClientContext context = (AdminPortClientContext)sender;
        }

        private void Context_MessageReceived(object sender, IAdminMessage e)
        {
            AdminPortClientContext context = (AdminPortClientContext)sender;
        }

        virtual protected void MessageReceived(AdminPortClientContext context, IAdminMessage message) { }

        virtual protected void ContextErrored(AdminPortClientContext context, IAdminMessage message) { }



    }
}
