using System;
using Akka.Actor;

namespace OpenTTDAdminPort.Events
{
    // Waits for event to be received and if received correct then returns it to specified party.
    public class AdminEventWaiterActor : ReceiveActor
    {
        private readonly Func<IAdminEvent, bool> func;
        private readonly IActorRef sender;

        public AdminEventWaiterActor(Func<IAdminEvent, bool> func, IActorRef sender)
        {
            this.func = func;
            this.sender = sender;
            Receive<IAdminEvent>(ReceiveAdminEvent);
        }

        public static Props Create(Func<IAdminEvent, bool> func, IActorRef sender)
            => Props.Create(() => new AdminEventWaiterActor(func, sender));

        public void ReceiveAdminEvent(IAdminEvent adminEvent)
        {
            if(func(adminEvent))
            {
                sender.Tell(adminEvent);
            }
        }
    }
}
