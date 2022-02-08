using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

using System;

namespace OpenTTDAdminPort.Networking.Watchdog
{
    internal class ConnectionWatchdog : ReceiveActor, IWithTimers
    {
        public event EventHandler<Exception>? Errored;

        private uint lastSendPingArg = 0;
        private bool lastPingReceived = true;
        private DateTimeOffset lastPingSentTime = DateTimeOffset.MinValue;
        private readonly TimeSpan maximumPingTime;

        private readonly IActorRef tcpClient;
        private readonly ILogger logger;
        private readonly IServiceScope scope;

        private readonly Random rand = new Random();

        // Initialized by Akka.NET
        public ITimerScheduler Timers { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionWatchdog"/> class.
        /// </summary>
        /// <param name="pingRespondTime">Specify how long will be time between pings to the server. 
        /// It is also specifying how long time does server have in order to respond to the message.</param>
        public ConnectionWatchdog(IServiceProvider sp, IActorRef tcpClient, TimeSpan maximumPingTime)
        {
            this.tcpClient = tcpClient;
            this.maximumPingTime = maximumPingTime;
            this.scope = sp.CreateScope();

            sp = scope.ServiceProvider;
            logger = sp.GetRequiredService<ILogger<ConnectionWatchdog>>();

            tcpClient.Tell(new TcpClientSubscribe());
            Ready();
        }

        public static Props Create(IServiceProvider sp, IActorRef tcpClient, TimeSpan maximumPingTime)
            => Props.Create(() => new ConnectionWatchdog(sp, tcpClient, maximumPingTime));

        protected override void PostStop()
        {
            tcpClient.Tell(new TcpClientUnsubscribe());
            scope.Dispose();
            base.PostStop();
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new SendPingMessage(lastSendPingArg), Self);
            base.PreStart();
        }

        private void Ready()
        {
            Receive<SendPingMessage>(msg =>
            {
                logger.LogTrace($"Received {nameof(SendPingMessage)} with {msg.Argument}");
                if (msg.Argument != lastSendPingArg)
                {
                    logger.LogTrace($"Return because {msg.Argument} != {lastSendPingArg}");
                    return;
                }

                if (lastPingReceived == false && (lastPingSentTime - DateTimeOffset.Now).Duration() > maximumPingTime)
                {
                    logger.LogTrace($"Connection lost as last pong {lastSendPingArg} for this argument was not received in specified time window. {lastPingSentTime.DateTime.ToTime()} - {DateTimeOffset.Now.DateTime.ToTime()} = {(lastPingSentTime - DateTimeOffset.Now).Duration().TotalSeconds} s > {maximumPingTime.TotalSeconds} s");
                    Context.Parent.Tell(new WatchdogConnectionLost());
                }
                else
                {
                    lastPingReceived = false;
                    lastPingSentTime = DateTimeOffset.Now;
                    lastSendPingArg = (uint)rand.Next(0, int.MaxValue);
                    tcpClient.Tell(new SendMessage(new AdminPingMessage(lastSendPingArg)));
                    Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new SendPingMessage(lastSendPingArg), Self);
                    logger.LogTrace($"sent ping message with {lastSendPingArg}");
                }
            });

            Receive<ReceiveMessage>(msg =>
            {
                if (msg.Message is AdminServerPongMessage pong)
                {
                    logger.LogTrace($"Received pong message {lastSendPingArg}");

                    if (pong.Argument == lastSendPingArg)
                    {
                        lastPingReceived = true;
                        Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new SendPingMessage(lastSendPingArg), Self);
                        logger.LogTrace("LastPingReceived = true");
                    }
                    else
                    {
                        logger.LogTrace("Message did not match ping arg");
                    }
                }
            });
        }
    }
}
