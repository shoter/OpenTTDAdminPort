using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public enum ContainerApplicationState
    {
        Idle = 1,
        Running = 2,
        Stopped = 3,
        Errored = 4,
    }
}
