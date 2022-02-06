using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
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

            IdleReady();
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
