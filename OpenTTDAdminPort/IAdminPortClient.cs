using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messaging;
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

        ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; }

        ConcurrentDictionary<uint, Player> Players { get; }

        AdminServerInfo AdminServerInfo { get; }

        event EventHandler<IAdminEvent> EventReceived;

        ServerInfo ServerInfo { get; }

        void SendMessage(IAdminMessage message);
        Task Join();
        Task Disconnect();
    }
}
