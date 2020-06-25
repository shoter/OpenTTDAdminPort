using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class AdminPortClient
    {
        private AdminPortClientContext Context { get; }

        private Dictionary<AdminConnectionState, IAdminPortClientState> StateRunners { get; } = new Dictionary<AdminConnectionState, IAdminPortClientState>();

        public AdminPortClient()
        {
            Context = new AdminPortClientContext("AdminPort", "1.0.0");
            //StateRunners[AdminConnectionState.Idle] = new AdminPortIdleState();
        }

        public Task Connect() => StateRunners[Context.State].Connect(Context);

        public Task Disconnect() => StateRunners[Context.State].Disconnect(Context);


        
    }
}
