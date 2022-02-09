using Akka.Actor;

using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Watchdog;

using System;
using System.IO;

namespace OpenTTDAdminPort.Akkas
{
    public class ActorFactory : IActorFactory
    {
        protected readonly IServiceProvider serviceProvider;

        public ActorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public virtual IActorRef CreateActor(IActorContext context, Func<IServiceProvider, Props> propsCreator)
        {
            Props props = propsCreator.Invoke(serviceProvider);
            return context.ActorOf(props);
        }

        public virtual IActorRef CreateReceiver(IActorContext context, Stream stream)
            => CreateActor(context, sp => AdminPortTcpClientReceiver.Create(sp, stream));

        public virtual IActorRef CreateTcpClient(IActorContext context, string ip, int port)
            => CreateActor(context, sp => AdminPortTcpClient.Create(sp, ip, port));

        public virtual IActorRef CreateWatchdog(IActorContext context, IActorRef tcpClient, TimeSpan maximumPingTime)
            => CreateActor(context, sp => ConnectionWatchdog.Create(sp, tcpClient, maximumPingTime));

    }
}
