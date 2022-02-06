using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortClientContext : IAdminPortClientContext
    {
        public IAdminPortTcpClient TcpClient { get; private set; }

        public event EventHandler<AdminConnectionStateChangedArgs>? StateChanged;

        public string ClientName { get; }
        public string ClientVersion { get; }
        public ServerInfo ServerInfo { get; }

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>();
        public ConcurrentDictionary<uint, Player> Players { get; } = new ConcurrentDictionary<uint, Player>();
        public AdminServerInfo? AdminServerInfo { get; set; }

        public IConnectionWatchdog WatchDog { get; } 


        public AdminConnectionState state = AdminConnectionState.Idle;
        public AdminConnectionState State
        {
            get => state;
            set
            {
                AdminConnectionState old = state;
                state = value;
                StateChanged?.Invoke(this, new AdminConnectionStateChangedArgs(old, value));
            }
        }
        public ConcurrentQueue<IAdminMessage> MessagesToSend { get; } = new ConcurrentQueue<IAdminMessage>();

        public AdminPortClientContext(IAdminPortTcpClient adminPortTcpClient, string clientName, string clientVersion, ServerInfo serverInfo, ILogger logger, AdminPortClientSettings settings)
        {
            this.ClientName = clientName;
            this.ClientVersion = clientVersion;
            this.State = AdminConnectionState.Idle;
            this.TcpClient = adminPortTcpClient;
            this.ServerInfo = serverInfo;
            this.WatchDog = new ConnectionWatchdog(settings.WatchdogInterval, logger);

            foreach (var updateType in Enums.ToArray<AdminUpdateType>())
            {
                this.AdminUpdateSettings.TryAdd(updateType, new AdminUpdateSetting(false, updateType, UpdateFrequency.ADMIN_FREQUENCY_POLL));
            }
        }
    }
}
