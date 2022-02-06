using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClient : FSM<AdminPortTcpClientState, IAdminPortTcpClientMessage>, IAdminPortTcpClient, IDisposable
    {
        internal void IdleState()
        {
            When(AdminPortTcpClientState.Idle, state =>
            {

                return null;
            });
        }
        
    }
}
