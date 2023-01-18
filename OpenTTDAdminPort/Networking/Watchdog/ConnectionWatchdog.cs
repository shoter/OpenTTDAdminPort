using System;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Networking.Watchdog
{
    internal class ConnectionWatchdog : ScopedReceiveActor, IWithTimers
    {
        private readonly TimeSpan maximumPingTime;
        private readonly IActorRef tcpClient;
        private readonly ILogger logger;
        private readonly Random rand = new();

        private uint lastSendPingArg = 0;
        private bool lastPingReceived = true;
        private DateTimeOffset lastPingSentTime = DateTimeOffset.MinValue;

        /// <remarks>
        /// Initialized by Akka.NET
        /// </remarks>
        public ITimerScheduler Timers { get; set; } = default!;

        private AdminPortClientSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionWatchdog"/> class.
        /// </summary>
        public ConnectionWatchdog(IServiceProvider sp, IActorRef tcpClient)
            : base(sp)
        {
            this.tcpClient = tcpClient;

            logger = SP.GetRequiredService<ILogger<ConnectionWatchdog>>();
            settings = SP.GetRequiredService<IOptions<AdminPortClientSettings>>().Value;
            this.maximumPingTime = settings.WatchdogInterval;

            tcpClient.Tell(new TcpClientSubscribe());
            Ready();
        }

        public static Props Create(IServiceProvider sp, IActorRef tcpClient, TimeSpan maximumPingTime)
            => Props.Create(() => new ConnectionWatchdog(sp, tcpClient));

        protected override void PostStop()
        {
            tcpClient.Tell(new TcpClientUnsubscribe());
            base.PostStop();
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new WatchdogHeartBeat(lastSendPingArg), Self);
            base.PreStart();
        }

        private void Ready()
        {
            Receive<WatchdogHeartBeat>(msg =>
            {
                logger.LogTrace($"Received {nameof(WatchdogHeartBeat)} with {msg.Argument}");
                if (msg.Argument != lastSendPingArg)
                {
                    logger.LogTrace($"Return because {msg.Argument} != {lastSendPingArg}");
                    return;
                }

                if (
                    !lastPingReceived &&
                    (lastPingSentTime - DateTimeOffset.Now).Duration() > maximumPingTime)
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
                    Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new WatchdogHeartBeat(lastSendPingArg), Self);
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
                        Context.System.Scheduler.ScheduleTellOnce(this.maximumPingTime, Self, new WatchdogHeartBeat(lastSendPingArg), Self);
                        logger.LogTrace("LastPingReceived = true");
                    }
                    else
                    {
                        logger.LogTrace("Message did not match ping arg. Ignoring");
                    }
                }
            });
        }
    }
}
