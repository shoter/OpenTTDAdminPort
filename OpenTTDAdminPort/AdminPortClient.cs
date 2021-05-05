using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class AdminPortClient : IAdminPortClient
    {
        private AdminPortClientContext Context { get; set; }

        private readonly IAdminEventFactory eventFactory;

        private Dictionary<AdminConnectionState, IAdminPortClientState> StateRunners { get; } = new Dictionary<AdminConnectionState, IAdminPortClientState>();

        public event EventHandler<IAdminEvent>? EventReceived;

        public AdminServerInfo? AdminServerInfo => Context?.AdminServerInfo;
        public ServerInfo ServerInfo => Context.ServerInfo;

        public AdminConnectionState ConnectionState => Context.State;

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings => Context.AdminUpdateSettings;

        public ConcurrentDictionary<uint, Player> Players => Context.Players;

        private readonly ILogger? logger;


        public AdminPortClient(ServerInfo serverInfo)
        {
            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService), new AdminPortTcpClientReceiver(packetService), new MyTcpClient());
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo);
            eventFactory = new AdminEventFactory();
            Init(tcpClient);
        }

        public AdminPortClient(ServerInfo serverInfo, ILogger<AdminPortClient> logger) : this(serverInfo)
        {
            this.logger = logger;
        }


        internal AdminPortClient(IAdminPortTcpClient adminPortTcpClient, IAdminEventFactory eventFactory, ServerInfo serverInfo)
        {
            Context = new AdminPortClientContext(adminPortTcpClient, "AdminPort", "1.0.0", serverInfo);
            this.eventFactory = eventFactory;
            Init(adminPortTcpClient);
        }

        private void Init(IAdminPortTcpClient adminPortTcpClient)
        {
            adminPortTcpClient.MessageReceived += AdminPortTcpClient_MessageReceived;
            adminPortTcpClient.Errored += AdminPortTcpClient_Errored;
            Context.StateChanged += Context_StateChanged;

            this.StateRunners[AdminConnectionState.Idle] = new AdminPortIdleState();
            this.StateRunners[AdminConnectionState.Connecting] = new AdminPortConnectingState();
            this.StateRunners[AdminConnectionState.Disconnecting] = new AdminPortDisconnectingState();
            this.StateRunners[AdminConnectionState.Connected] = new AdminPortConnectedState();
            this.StateRunners[AdminConnectionState.Errored] = new AdminPortErroredState();
            this.StateRunners[AdminConnectionState.ErroredOut] = new AdminPortErroredOutState();
        }

        private void AdminPortTcpClient_Errored(object sender, Exception e)
        {
            logger.LogError(e, $"TCP client had internal error {e.Message}.");
            Context.state = AdminConnectionState.Errored;
        }

        private void AdminPortTcpClient_MessageReceived(object sender, IAdminMessage e)
        {
            StateRunners[Context.State].OnMessageReceived(e, Context);
            IAdminEvent? adminEvent = eventFactory.Create(e, Context);
            if(adminEvent != null)
                EventReceived?.Invoke(this, adminEvent);
        }

        private void Context_StateChanged(object sender, AdminConnectionStateChangedArgs e)
        {
            try
            {
                StateRunners[e.Old].OnStateEnd(Context);
                StateRunners[e.New].OnStateStart(Context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error during changing state from {e.Old} into {e.New}");
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        public async Task Connect()
        {
            try
            {
                await StateRunners[Context.State].Connect(Context);

                if (!(await TaskHelper.WaitUntil(() => Context.State == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    throw new AdminPortException("Admin port could not connect to the server");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Connect");
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await StateRunners[Context.State].Disconnect(Context);

                if (!(await TaskHelper.WaitUntil(() => Context.State == AdminConnectionState.Idle, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    throw new AdminPortException($"Encountered internal error. Expected state {AdminConnectionState.Idle} but got {Context.State}."); 
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Connect");
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        public void SendMessage(IAdminMessage message)
        {
            try
            {
                StateRunners[Context.State].SendMessage(message, Context);
            }
            catch (Exception)
            {
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }
    }
}
