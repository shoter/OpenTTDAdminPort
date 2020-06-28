using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    internal interface IConnectionWatchdog
    {
        event EventHandler<Exception>? Errored;
        void Start(IAdminPortTcpClient client);
        void Stop();

        public bool Enabled { get; }
    }
}
