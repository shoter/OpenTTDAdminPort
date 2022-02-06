using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;

using System;
using System.IO;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClient : ReceiveActor
    {
        private ITcpClient tcpClient;
        private readonly IAdminPacketService adminPacketService;
        private readonly IActorFactory actorFactory;
        private readonly IServiceScope scope;

        // Obtained after connect
        private Stream? stream;
        private IActorRef? receiver;

        private readonly ILogger logger;

        public AdminPortTcpClient(IServiceProvider serviceProvider)
        {
            this.scope = serviceProvider.CreateScope();
            serviceProvider = this.scope.ServiceProvider;
            this.tcpClient = serviceProvider.GetRequiredService<ITcpClient>();
            this.adminPacketService = serviceProvider.GetRequiredService<IAdminPacketService>();
            this.actorFactory = serviceProvider.GetRequiredService<IActorFactory>();
            this.logger = serviceProvider.GetRequiredService<ILogger<AdminPortTcpClient>>();

            this.stream = tcpClient.GetStream();
            this.receiver = actorFactory.CreateActor(Context, sp => Props.Create(() => new AdminPortTcpClientReceiver(sp, stream)));

            Ready();
        }

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

            Receive<AdminPortTcpClientDisconnect>(_ =>
            {
                receiver.Tell(PoisonPill.Instance);
                receiver = null;

                tcpClient.Dispose();
                stream = null;
                tcpClient = scope.ServiceProvider.GetRequiredService<ITcpClient>();

                UnbecomeStacked();
            });

            Receive<ReceiveMessage>(receiveMessage =>
            {
                Context.Parent.Tell(receiveMessage);
            });
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
                        case ReceiveLoopException:
                            return Directive.Escalate;
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
