using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal interface IAdminPortClientState
    {
        void OnStateStart(IAdminPortClientContext context);
        Task Connect(IAdminPortClientContext context);
        Task Disconnect(IAdminPortClientContext context);
        void OnStateEnd(IAdminPortClientContext context);
        void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context);
       

    }
}
