using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;

using System;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>
    {

        private readonly IActorFactory actorFactory;

        private readonly string ip;
        private readonly int port;

        private readonly string version;

        private readonly IServiceScope scope;

        public AdminPortClientActor(IServiceProvider sp, string ip, int port)
        {
            this.ip = ip;
            this.port = port;

            this.scope = sp.CreateScope();
            sp = this.scope.ServiceProvider;

            this.actorFactory = sp.GetRequiredService<IActorFactory>();
            this.version = "1.0.0";

        }

        public void Ready()
        {
            StartWith(MainState.Idle, new IdleData());
            IdleState();

        }

        protected override void PostStop()
        {

            base.PostStop();
        }
    }
}
