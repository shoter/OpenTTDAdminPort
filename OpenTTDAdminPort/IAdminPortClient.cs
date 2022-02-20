using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public interface IAdminPortClient
    {
        AdminConnectionState ConnectionState { get; }


        event EventHandler<IAdminEvent> EventReceived;

        ServerInfo ServerInfo { get; }

        void SendMessage(IAdminMessage message);
        Task Connect();
        Task Disconnect();
    }
}
