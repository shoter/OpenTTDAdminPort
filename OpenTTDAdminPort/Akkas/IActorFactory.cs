using Akka.Actor;

using System;

namespace OpenTTDAdminPort.Akkas
{
    public interface IActorFactory
    {
        IActorRef CreateActor(IActorContext context, Func<IServiceProvider, Props> propsCreator);

    }
}
