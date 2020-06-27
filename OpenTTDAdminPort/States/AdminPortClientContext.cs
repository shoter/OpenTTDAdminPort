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
    public class AdminPortClientContext
    {
        internal IAdminPortTcpClient TcpClient { get; private set; }

        internal event EventHandler<IAdminMessage>? MessageReceived;
        internal event EventHandler<AdminConnectionStateChangedArgs>? StateChanged;

        internal string ClientName { get; }
        internal string ClientVersion { get; }
        internal ServerInfo ServerInfo { get; }

        internal ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>();
        internal ConcurrentDictionary<uint, Player> Players { get; } = new ConcurrentDictionary<uint, Player>();
        internal AdminServerInfo AdminServerInfo { get; set; }


        private AdminConnectionState state = AdminConnectionState.Idle;
        internal AdminConnectionState State
        {
            get => state;
            set
            {
                AdminConnectionState old = state;
                state = value;
                StateChanged?.Invoke(this, new AdminConnectionStateChangedArgs(old, value));
            }
        }
        internal ConcurrentQueue<IAdminMessage> MessagesToSend { get; } = new ConcurrentQueue<IAdminMessage>();

        public AdminPortClientContext(IAdminPortTcpClient adminPortTcpClient, string clientName, string clientVersion, ServerInfo serverInfo)
        {
            this.ClientName = clientName;
            this.ClientVersion = clientVersion;
            this.State = AdminConnectionState.Idle;
            this.TcpClient = adminPortTcpClient;
            this.ServerInfo = serverInfo;

            this.TcpClient.MessageReceived += (who, e) => this.MessageReceived?.Invoke(this, e);
        }
    }
}
