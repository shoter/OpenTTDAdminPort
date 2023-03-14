using System;
using System.Collections.Generic;
using System.IO;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking.Exceptions;
using OpenTTDAdminPort.Packets;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClient : ReceiveActor
    {
        private readonly IAdminPacketService adminPacketService;
        private readonly IActorFactory actorFactory;
        private readonly IServiceScope scope;
        private readonly HashSet<IActorRef> subscribers = new();
        private readonly ILogger logger;

        // Obtained after connect
        private ITcpClient tcpClient;
        private Stream? stream;
        private IActorRef? receiver;

        public AdminPortTcpClient(IServiceProvider serviceProvider, string ip, int port)
        {
            this.scope = serviceProvider.CreateScope();
            serviceProvider = this.scope.ServiceProvider;
            this.tcpClient = serviceProvider.GetRequiredService<ITcpClient>();
            this.adminPacketService = serviceProvider.GetRequiredService<IAdminPacketService>();
            this.actorFactory = serviceProvider.GetRequiredService<IActorFactory>();
            this.logger = serviceProvider.GetRequiredService<ILogger<AdminPortTcpClient>>();

            try
            {
                tcpClient.ConnectAsync(ip, port).Wait();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Connection failed");
                throw new InitialConnectionException("Connection failed", ex);
            }

            this.stream = tcpClient.GetStream();
            this.receiver = actorFactory.CreateReceiver(Context, stream);

            Ready();
        }

        public static Props Create(IServiceProvider sp, string ip, int port)
            => Props.Create(() => new AdminPortTcpClient(sp, ip, port));

        private void Ready()
        {
            ReceiveAsync<SendMessage>(async sendMessage =>
            {
                try
                {
                    IAdminMessage msg = sendMessage.Message;

                    logger.LogTrace($"Sender sending {msg}!");
                    Packet packet = this.adminPacketService.CreatePacket(msg);
                    await stream!.WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));
                    logger.LogTrace($"Sender sent {msg}!");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Sender errored");
                    throw new SendException("Message sending failed", e);
                }
            });

            Receive<ReceiveMessage>(receiveMessage =>
            {
                logger.LogTrace($"Received {receiveMessage}");
                Context.Parent.Tell(receiveMessage);
                foreach (var s in subscribers)
                {
                    s.Tell(receiveMessage);
                }
            });

            Receive<TcpClientSubscribe>(_ => subscribers.Add(Sender));
            Receive<TcpClientUnsubscribe>(_ => subscribers.Remove(Sender));
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case ReceiveLoopException e:
                            // Will cause restart of tcp client through supervisor of main actor.
                            logger.LogError("Fatal exception of TcpClient detected. Scheduling restart of it in 3 seconds");
                            Context.Parent.Tell(new AdminPortTcpClientConnectionLostException(e.Message, e));
                            return Directive.Stop;
                        default:
                            return Directive.Restart;
                    }
                });
        }

        protected override void PostStop()
        {
            tcpClient.Dispose();
            scope.Dispose();
            base.PostStop();
        }
    }
}
