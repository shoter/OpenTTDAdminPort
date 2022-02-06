using Akka.Actor;

using OpenTTDAdminPort.Akkas;

using System;
using System.IO;

namespace OpenTTDAdminPort.Networking
{
    public class NetworkingActorFactory : ActorFactory, INetworkingActorFactory
    {
        public NetworkingActorFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public virtual IActorRef CreateReceiver(IActorContext context, Stream stream) => CreateActor(context, sp => AdminPortTcpClientReceiver.Create(sp, stream));
    }
}
