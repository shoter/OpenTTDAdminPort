using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    public interface IAdminPortTcpClient
    {
        event EventHandler<IAdminMessage> MessageReceived;
        event EventHandler<Exception> Errored;
        void SendMessage(IAdminMessage message);
        Task Start();
        Task Stop();

        WorkState State { get; }
    }
}
