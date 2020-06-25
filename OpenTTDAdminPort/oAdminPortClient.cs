//using Microsoft.Extensions.Logging;
//using OpenTTDAdminPort.Common;
//using OpenTTDAdminPort.Events;
//using OpenTTDAdminPort.Messages;
//using OpenTTDAdminPort.Networking;
//using System;
//using System.Collections.Concurrent;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;
//using OpenTTDAdminPort.Packets;
//using OpenTTDAdminPort.Game;

//namespace OpenTTDAdminPort
//{
//    public class oAdminPortClient : IAdminPortClient
//    {
//        private TcpClient? tcpClient;
//        public AdminConnectionState ConnectionState { get; private set; }

//        public ConcurrentDictionary<uint, Player> Players { get; } = new ConcurrentDictionary<uint, Player>();

//        public event EventHandler<IAdminEvent>? EventReceived;

//        private readonly Microsoft.Extensions.Logging.ILogger? logger;

//        private readonly IAdminPacketService adminPacketService;

//        private readonly IAdminMessageProcessor messageProcessor;

//        private readonly ConcurrentQueue<IAdminMessage> receivedMessagesQueue = new ConcurrentQueue<IAdminMessage>();

//        private readonly ConcurrentQueue<IAdminMessage> sendMessageQueue = new ConcurrentQueue<IAdminMessage>();

//        private DateTime lastMessageSentTime = DateTime.Now;
//        private DateTime lastMessageReceivedTime = DateTime.Now;
//        private Mutex startMutex = new Mutex();

//        private readonly string clientName;
//        private readonly string clientVersion;



//        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

//        public ServerInfo ServerInfo { get; }

//        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>();


//        public AdminServerInfo AdminServerInfo { get; private set; } = new AdminServerInfo();
//        internal oAdminPortClient(ServerInfo serverInfo, IAdminMessageProcessor messageProcessor, ILogger<IAdminPortClient>? logger)
//        {
//            var adminPacketServiceFactory = new AdminPacketServiceFactory();

//            this.ServerInfo = serverInfo;
//            this.logger = logger;
//            this.adminPacketService = adminPacketServiceFactory.Create();
//            this.messageProcessor = messageProcessor;
//            this.clientName = "AdminPortClient_csharp";
//            this.clientVersion = "1.0";

//            foreach (var type in Enums.ToArray<AdminUpdateType>())
//            {
//                this.AdminUpdateSettings.TryAdd(type, new AdminUpdateSetting(false, type, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
//            }
//        }

//        public oAdminPortClient(ServerInfo serverInfo, ILogger<IAdminPortClient>? logger = null)
//            : this(serverInfo, new AdminMessageProcessor(), logger)
//        {
//        }

//        private async void EventLoop(CancellationToken token)
//        {
//            while (token.IsCancellationRequested == false)
//            {
//                if (this.receivedMessagesQueue.TryDequeue(out IAdminMessage msg))
//                {
//                    var eventMessage = this.messageProcessor.ProcessMessage(msg, this);

//                    if (eventMessage != null)
//                        this.EventReceived?.Invoke(this, eventMessage);
//                }

//                await Task.Delay(TimeSpan.FromSeconds(0.1));
//            }
//        }

//        private async void MainLoop(CancellationToken token)
//        {
//            Task<int>? sizeTask = null;
//            byte[] sizeBuffer = new byte[2];

//            while (token.IsCancellationRequested == false)
//            {
//                try
//                {
//                    if (this.ConnectionState == AdminConnectionState.NotConnected)
//                    {
//                        tcpClient = new TcpClient();
//                        lastMessageSentTime = DateTime.Now;
//                        lastMessageReceivedTime = DateTime.Now;
//                        tcpClient.Connect(ServerInfo.ServerIp, ServerInfo.ServerPort);
//                        this.SendMessage(new AdminJoinMessage(ServerInfo.Password, clientName, clientVersion));
//                        logger.LogInformation($"{ServerInfo} Connecting");

//                        this.ConnectionState = AdminConnectionState.Connecting;
//                    }

//                    if (this.tcpClient == null)
//                        continue;

//                    if ((DateTime.Now - lastMessageSentTime) > TimeSpan.FromSeconds(10))
//                    {
//                        this.SendMessage(new AdminPingMessage());
//                    }

//                    if (DateTime.Now - lastMessageReceivedTime > TimeSpan.FromMinutes(1))
//                    {
//                        throw new AdminPortException("No messages received for 60 seconds!");
//                    }

//                    for (int i = 0; i < 100; ++i)
//                    {
//                        if (this.sendMessageQueue.TryDequeue(out IAdminMessage msg))
//                        {
//                            logger.LogInformation($"{ServerInfo} sent {msg.MessageType}");
//                            Packet packet = this.adminPacketService.CreatePacket(msg);
//                            await tcpClient.GetStream().WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));
//                            lastMessageSentTime = DateTime.Now;
//                        }
//                        else
//                            break;
//                    }

//                    while ((sizeTask ??= tcpClient.GetStream().ReadAsync(sizeBuffer, 0, 2)).IsCompleted)
//                    {
//                        var receivedBytes = sizeTask.Result;

//                        if (receivedBytes != 2)
//                        {
//                            await Task.Delay(TimeSpan.FromMilliseconds(1));
//                            int bytes = await tcpClient.GetStream().ReadAsync(sizeBuffer, 1, 1).WaitMax(TimeSpan.FromSeconds(2));
//                            if (bytes == 0)
//                            {
//                                throw new AdminPortException("Something went wrong - restarting");
//                            }

//                        }

//                        sizeTask = null;

//                        ushort size = BitConverter.ToUInt16(sizeBuffer, 0);

//                        byte[] content = new byte[size];
//                        content[0] = sizeBuffer[0];
//                        content[1] = sizeBuffer[1];
//                        int contentSize = 2;

//                        lastMessageReceivedTime = DateTime.Now;

//                        do
//                        {
//                            await Task.Delay(TimeSpan.FromMilliseconds(1));
//                            Task<int> task = tcpClient.GetStream().ReadAsync(content, contentSize, size - contentSize).WaitMax(TimeSpan.FromSeconds(2), $"{ServerInfo} no data received");
//                            await task;
//                            contentSize += task.Result;
//                            if (task.Result == 0)
//                            {
//                                throw new AdminPortException("No further data received in message!");
//                            }
//                        } while (contentSize < size);


//                        var packet = new Packet(content);
//                        IAdminMessage message = this.adminPacketService.ReadPacket(packet);
//                        if (message == null)
//                            break;

//                        this.logger.LogInformation($"{ServerInfo} received {message.MessageType}");

//                        switch (message.MessageType)
//                        {
//                            case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
//                                {
//                                    var msg = (AdminServerProtocolMessage)message;

//                                    foreach (var s in msg.AdminUpdateSettings)
//                                    {
//                                        this.logger.LogInformation($"Update settings {s.Key} - {s.Value}");
//                                        this.AdminUpdateSettings.TryUpdate(s.Key, new AdminUpdateSetting(true, s.Key, s.Value), this.AdminUpdateSettings[s.Key]);
//                                    }

//                                    break;
//                                }
//                            case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
//                                {
//                                    var msg = (AdminServerWelcomeMessage)message;

//                                    AdminServerInfo = new AdminServerInfo()
//                                    {
//                                        IsDedicated = msg.IsDedicated,
//                                        MapName = msg.MapName,
//                                        RevisionName = msg.NetworkRevision,
//                                        ServerName = msg.ServerName
//                                    };


//                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CHAT, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
//                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CONSOLE, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
//                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
//                                    this.SendMessage(new AdminPollMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, uint.MaxValue));

//                                    this.ConnectionState = AdminConnectionState.Connected;
//                                    this.logger.LogInformation($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} - connected");

//                                    break;
//                                }
//                            case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO:
//                                {
//                                    var msg = (AdminServerClientInfoMessage)message;
//                                    var player = new Player(msg.ClientId, msg.ClientName);
//                                    Players.AddOrUpdate(msg.ClientId, player, (_, __) => player);

//                                    break;
//                                }
//                            case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE:
//                                {
//                                    var msg = (AdminServerClientUpdateMessage)message;
//                                    var player = Players[msg.ClientId];
//                                    player.Name = msg.ClientName;

//                                    break;
//                                }
//                            default:
//                                {
//                                    var msg = (AdminServerChatMessage)message;
//                                    this.receivedMessagesQueue.Enqueue(message);
//                                    break;
//                                }
//                        }

//                    }



//                    await Task.Delay(TimeSpan.FromSeconds(0.5));
//                }
//                catch (Exception e)
//                {
//                    this.logger.LogError($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} encountered error {e.Message}", e);

//                    this.tcpClient?.Dispose();
//                    this.tcpClient = null;
//                    this.sendMessageQueue.Clear();
//                    this.receivedMessagesQueue.Clear();
//                    sizeTask = null;
//                    this.ConnectionState = AdminConnectionState.NotConnected;

//                    await Task.Delay(TimeSpan.FromSeconds(60));
//                }

//            }

//            this.logger.LogInformation($"{ServerInfo} disconnected");
//            this.ConnectionState = AdminConnectionState.Idle;
//        }

//        public async Task Join()
//        {
//            if (this.ConnectionState != AdminConnectionState.Idle)
//                return;

//            lock (startMutex)
//            {
//                if (this.ConnectionState == AdminConnectionState.Idle)
//                {
//                    this.ConnectionState = AdminConnectionState.NotConnected;
//                }
//                else
//                {
//                    return;
//                }
//            }

//            try
//            {
//                this.cancellationTokenSource = new CancellationTokenSource();

//                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token)), null);
//                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => EventLoop(cancellationTokenSource.Token)), null);

//                if (!(await TaskHelper.WaitUntil(() => ConnectionState == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
//                {
//                    this.cancellationTokenSource.Cancel();
//                    this.cancellationTokenSource = new CancellationTokenSource();
//                    throw new AdminPortException("Admin port could not connect to the server");
//                }
//            }
//            catch (Exception e)
//            {
//                this.ConnectionState = AdminConnectionState.Idle;
//                throw new AdminPortException("Could not join server", e);
//            }
//        }

//        public async Task Disconnect()
//        {
//            if (this.ConnectionState == AdminConnectionState.Idle)
//                return;
//            try
//            {
//                this.cancellationTokenSource.Cancel();

//                if (!(await TaskHelper.WaitUntil(() => ConnectionState == AdminConnectionState.Idle, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
//                {
//                    this.cancellationTokenSource = new CancellationTokenSource();
//                }
//            }
//            catch (Exception e)
//            {
//                throw new AdminPortException("Error during stopping server", e);
//            }
//        }

//        public void SendMessage(IAdminMessage message)
//        {
//            this.sendMessageQueue.Enqueue(message);
//        }
//    }
//}
