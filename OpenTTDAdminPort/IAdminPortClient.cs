using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort
{
    public interface IAdminPortClient
    {
        AdminConnectionState ConnectionState { get; }

        ServerInfo ServerInfo { get; }

        void SendMessage(IAdminMessage message);

        void SetAdminEventHandler(Action<IAdminEvent> action);

        Task Connect(ILogger? test = null);

        Task Disconnect();
    }
}
