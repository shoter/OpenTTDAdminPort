using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClient : FSM<WorkState, IAdminPortTcpClientMessage>, IAdminPortTcpClient, IDisposable
    {
        internal void IdleState()
        {
            When(WorkState.Idle, state =>
            {
                if(state.FsmEvent is AdminPortTcpClientConnect connect)
                {
                    tcpClient.ConnectAsync(connect.Ip, connect.Port).Wait();
                }

                return null;
            });
        }
        
    }
}
