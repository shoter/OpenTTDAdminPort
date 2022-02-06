//using Microsoft.Extensions.Logging;

//using OpenTTDAdminPort.Messages;
//using OpenTTDAdminPort.Networking;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;
//using System.Timers;

//namespace OpenTTDAdminPort
//{
//    class ConnectionWatchdog : IConnectionWatchdog
//    {
//        public event EventHandler<Exception>? Errored;

//        private uint lastSendPingArg = 0;
//        private bool lastPingReceived = true;
//        private IAdminPortTcpClient? client;
//        private readonly ILogger logger;

//        private readonly Random rand = new Random();
//        private readonly Timer timer;

//        public bool Enabled => timer.Enabled;


//        /// <summary>
//        /// Initializes a new instance of the <see cref="ConnectionWatchdog"/> class.
//        /// </summary>
//        /// <param name="pingRespondTime">Specify how long will be time between pings to the server. 
//        /// It is also specifying how long time does server have in order to respond to the message.</param>
//        public ConnectionWatchdog(TimeSpan pingRespondTime, ILogger logger)
//        {
//            timer = new Timer(pingRespondTime.TotalMilliseconds);
//            timer.Elapsed += Timer_Elapsed;
//            this.logger = logger;
//        }


//        private void OnMessageReceived(object who, IAdminMessage message)
//        {
//            if(message.MessageType == AdminMessageType.ADMIN_PACKET_SERVER_PONG)
//            {
                
//                var pongMsg = (AdminServerPongMessage)message;
//                lastPingReceived = lastPingReceived || pongMsg.Argument == lastSendPingArg;
//                logger.LogTrace($"Watchdog received ping {pongMsg.Argument} == {lastSendPingArg} ({lastPingReceived})");

//            }
//        }

//        public void Start(IAdminPortTcpClient client)
//        {
//            if(timer.Enabled && this.client != client)
//            {
//                throw new AdminPortException("You cannot start timer with different instance of client when it is running!");
//            }
//            if(!timer.Enabled)
//            {
//                this.client = client;
//                client.MessageReceived += OnMessageReceived;
//                lastPingReceived = true;
//                timer.Start();
//            }
//        }

//        public void Stop()
//        {
//            if(timer.Enabled)
//            {
//                Debug.Assert(client != null);

//                lastPingReceived = true;
//                client.MessageReceived -= OnMessageReceived;
//                timer.Stop();
//            }
//        }

//        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            logger.LogTrace($"{DateTime.Now:hh mm ss}. Timer elapsed for watchdog {lastPingReceived}");
//            if (lastPingReceived == false)
//            {
//                logger.LogTrace("Invoking errored state on watchdog due to ping received in timely manner");
//                Errored?.Invoke(this, new AdminPortException("Server did not respond in predefined time"));
//                Stop();
//            }
//            else
//            {
//                SendPingMessage();
//            }
//        }


//        private void SendPingMessage()
//        {
//            lastSendPingArg = (uint)rand.Next(0, int.MaxValue);
//            lastPingReceived = false;
//            var pingMsg = new AdminPingMessage(lastSendPingArg);
//            client?.SendMessage(pingMsg);
//            logger.LogTrace($"Watchdog sent ping {lastSendPingArg} ({lastPingReceived})");

//        }
//    }
//}
