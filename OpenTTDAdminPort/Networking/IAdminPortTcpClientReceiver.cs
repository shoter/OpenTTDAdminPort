using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    public interface IAdminPortTcpClientReceiver
    {
        event EventHandler<IAdminMessage>? MessageReceived;
        event EventHandler<Exception>? ErrorOcurred;
        WorkState State { get; }

        Task Start(Stream stream);
        Task Stop();
    }
}
