using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    public interface IAdminPortTcpClientSender
    {
        event EventHandler<Exception>? ErrorOcurred;

        Task Start(Stream stream);
        Task Stop();
        void SendMessage(IAdminMessage message);
        WorkState State { get; }
    }
}
