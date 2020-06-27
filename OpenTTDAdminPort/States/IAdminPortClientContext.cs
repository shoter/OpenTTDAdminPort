using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal interface IAdminPortClientContext
    {
        IAdminPortTcpClient TcpClient { get; }

        string ClientName { get; }
        string ClientVersion { get; }
        ServerInfo ServerInfo { get; }

        event EventHandler<AdminConnectionStateChangedArgs>? StateChanged;

        ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } 
        ConcurrentDictionary<uint, Player> Players { get; }
        AdminServerInfo AdminServerInfo { get; set; }

        AdminConnectionState State { get; set; }

        ConcurrentQueue<IAdminMessage> MessagesToSend { get; }
    }
}
