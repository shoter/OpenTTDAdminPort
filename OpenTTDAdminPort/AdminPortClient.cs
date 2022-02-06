﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Logging;
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
        public event EventHandler<AdminConnectionStateChangedArgs>? StateChanged;

        public AdminServerInfo? AdminServerInfo => Context?.AdminServerInfo;
        public ServerInfo ServerInfo => Context.ServerInfo;

        public AdminConnectionState ConnectionState => Context.State;

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings => Context.AdminUpdateSettings;

        public ConcurrentDictionary<uint, Player> Players => Context.Players;

        private readonly ILogger? logger;


        public AdminPortClient(AdminPortClientSettings settings, ServerInfo serverInfo)
        {
            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService), new AdminPortTcpClientReceiver(packetService), new MyTcpClient());
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo, NullLogger.Instance, settings);
            eventFactory = new AdminEventFactory();
            Init(tcpClient);
        }

        public AdminPortClient(AdminPortClientSettings settings, ServerInfo serverInfo, ILogger<AdminPortClient> logger)
        {
            this.logger = logger;

            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService, logger), new AdminPortTcpClientReceiver(packetService, logger), new MyTcpClient(), logger);
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo, logger, settings);
            eventFactory = new AdminEventFactory();
            Init(tcpClient);

        }


        internal AdminPortClient(IAdminPortTcpClient adminPortTcpClient, IAdminEventFactory eventFactory, AdminPortClientSettings settings, ServerInfo serverInfo)
        {
            Context = new AdminPortClientContext(adminPortTcpClient, "AdminPort", "1.0.0", serverInfo, NullLogger.Instance, settings);
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

            logger?.LogInformation($"{ServerInfo} Admin Port Client Initialized.");
        }

        private void AdminPortTcpClient_Errored(object sender, Exception e)
        {
            logger?.LogError(e, $"{ServerInfo} TCP client had internal error: {e.Message}.");
            Context.State = AdminConnectionState.Errored;
        }

        private void AdminPortTcpClient_MessageReceived(object sender, IAdminMessage e)
        {
            try
            {
                logger?.LogTrace($"{ServerInfo} Received message {e.MessageType} - {e}");
                StateRunners[Context.State].OnMessageReceived(e, Context);
                IAdminEvent? adminEvent = eventFactory.Create(e, Context);
                logger?.LogWarning($"{ServerInfo} adminEvent is null for {e.MessageType} {e}");
                if (adminEvent != null)
                {
                    EventReceived?.Invoke(this, adminEvent);
                    logger?.LogTrace($"{ServerInfo} Created admin event {adminEvent.EventType} - {adminEvent}");
                }
            }
            catch(Exception ex)
            {
                logger?.LogError(ex, "Could not complete AdminPortTcpClient_MessageReceived");
            }
        }

        private void Context_StateChanged(object sender, AdminConnectionStateChangedArgs e)
        {
            try
            {
                logger?.LogTrace($"{ServerInfo} State changed from {e.Old} to {e.New}.");
                StateRunners[e.Old].OnStateEnd(Context);
                StateRunners[e.New].OnStateStart(Context);
                this.StateChanged?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"{ServerInfo} Error during changing state from {e.Old} into {e.New}");
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
                logger?.LogError(ex, $"{ServerInfo}Error during Connect");
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
                logger?.LogError(ex, $"{ServerInfo} Error during Connect");
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
