using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    public interface IAdminPortClientState
    {
        virtual Task OnStateStart(AdminPortClientContext context) { return Task.CompletedTask; }
        Task Connect(AdminPortClientContext context);
        Task Disconnect(AdminPortClientContext context);
        virtual Task OnMainLoopTick(AdminPortClientContext context) { return Task.CompletedTask; }
        virtual Task OnStateEnd(AdminPortClientContext context) { return Task.CompletedTask; }

    }
}
