using Akka.Actor;

using OpenTTDAdminPort.MainActor.Messages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>
    {
        public void ConnectedState()
        {

        }

    }
}
