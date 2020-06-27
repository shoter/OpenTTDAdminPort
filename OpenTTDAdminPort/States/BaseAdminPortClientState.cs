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

        virtual public void OnStateEnd(IAdminPortClientContext context)
        {
            context.MessageReceived += Context_MessageReceived;
        }

        virtual public void OnStateStart(IAdminPortClientContext context)
        {
            context.MessageReceived -= Context_MessageReceived;
        }

        private void Context_Errored(object sender, Exception e)
        {
            IAdminPortClientContext context = (IAdminPortClientContext)sender;
        }

        private void Context_MessageReceived(object sender, IAdminMessage e)
        {
            IAdminPortClientContext context = (IAdminPortClientContext)sender;
        }

        virtual protected void MessageReceived(IAdminPortClientContext context, IAdminMessage message) { }

        virtual protected void ContextErrored(IAdminPortClientContext context, IAdminMessage message) { }



    }
}
