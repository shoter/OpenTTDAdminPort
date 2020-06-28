using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal abstract class BaseAdminPortClientState : IAdminPortClientState
    {
        public abstract Task Connect(IAdminPortClientContext context);

        public abstract Task Disconnect(IAdminPortClientContext context);

        [ExcludeFromCodeCoverage]
        virtual public void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context)
        {
        }

        virtual public void SendMessage(IAdminMessage message, IAdminPortClientContext context)
        {
            context.MessagesToSend.Enqueue(message);
        }

        [ExcludeFromCodeCoverage]
        virtual public void OnStateEnd(IAdminPortClientContext context)
        {
        }

        [ExcludeFromCodeCoverage]
        virtual public void OnStateStart(IAdminPortClientContext context)
        {
        }
    }
}
