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
        void OnStateStart(AdminPortClientContext context);
        Task Connect(AdminPortClientContext context);
        Task Disconnect(AdminPortClientContext context);
        void OnStateEnd(AdminPortClientContext context);
        void OnMessageReceived(IAdminMessage message, AdminPortClientContext context);
       

    }
}
