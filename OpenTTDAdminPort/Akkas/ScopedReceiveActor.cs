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

        protected override void PostStop()
        {
            Scope.Dispose();
            base.PostStop();
        }
    }
}
