using Akka.Actor;

using System;

namespace OpenTTDAdminPort.Akkas
{
    public class ActorFactory : IActorFactory
    {
        protected readonly IServiceProvider serviceProvider;

        public ActorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IActorRef CreateActor(IActorContext context, Func<IServiceProvider, Props> propsCreator)
        {
            Props props = propsCreator.Invoke(serviceProvider);
            return context.ActorOf(props);
        }
    }
}
