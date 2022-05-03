using Akka.Actor;

using System;
using System.IO;

namespace OpenTTDAdminPort.Akkas
{
    public interface IActorFactory
    {
        IActorRef CreateActor(IActorContext context, Func<IServiceProvider, Props> propsCreator, string? name);

        IActorRef CreateMainActor(ActorSystem actorSystem);

        IActorRef CreateReceiver(IActorContext context, Stream stream);

        IActorRef CreateWatchdog(IActorContext context, IActorRef tcpClient, TimeSpan maximumPingTime);

        IActorRef CreateTcpClient(IActorContext context, string ip, int port);

        IActorRef CreateMessager(IActorContext context);
    }
}
