using System;
using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTTDAdminPort.Akkas
{
    internal class ScopedReceiveActor : ReceiveActor
    {
        protected IServiceScope Scope { get; }

        protected IServiceProvider SP => Scope.ServiceProvider;

        public ScopedReceiveActor(IServiceProvider sp)
        {
            Scope = sp.CreateScope();
        }

        public override void AroundPreRestart(Exception cause, object message)
        {
            if (cause != null && Sender != null)
            {
                Sender.Tell(cause);
            }

            base.AroundPreRestart(cause, message);
        }

        protected override void PostStop()
        {
            Scope.Dispose();
            base.PostStop();
        }
    }
}
