using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    public interface IAdminPortTcpClientSender
    {
        Task Start();
        Task Stop();
        void SendMessage(IAdminMessage message);
        WorkState State { get; }
    }
}
