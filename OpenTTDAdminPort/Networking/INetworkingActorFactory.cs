using Akka.Actor;

using OpenTTDAdminPort.Akkas;

using System.IO;

namespace OpenTTDAdminPort.Networking
{
    public interface INetworkingActorFactory : IActorFactory
    {
        IActorRef CreateReceiver(IActorContext context, Stream stream);

    }
}
