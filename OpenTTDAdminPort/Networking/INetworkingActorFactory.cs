using Akka.Actor;

using OpenTTDAdminPort.Akkas;

using System;
using System.IO;

namespace OpenTTDAdminPort.Networking
{
    public interface INetworkingActorFactory : IActorFactory
    {
        IActorRef CreateReceiver(IActorContext context, Stream stream);

        IActorRef CreateWatchdog(IActorContext context, IActorRef tcpClient, TimeSpan maximumPingTime);
    }
}
