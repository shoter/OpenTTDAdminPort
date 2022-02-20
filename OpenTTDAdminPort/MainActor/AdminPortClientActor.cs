using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;

using System;
using System.Collections.Generic;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash
    {

        private readonly IActorFactory actorFactory;

        private readonly string version;

        private readonly IServiceScope scope;

        private readonly ILogger logger;

        // Initialized by Akka.net
        public IStash Stash { get; set; } = default!;

        public HashSet<IActorRef> Subscribers { get; } = new();

        public AdminPortClientActor(IServiceProvider sp)
        {
            this.scope = sp.CreateScope();
            sp = this.scope.ServiceProvider;

            this.logger = sp.GetRequiredService<ILogger<AdminPortClientActor>>();

            this.actorFactory = sp.GetRequiredService<IActorFactory>();
            this.version = "1.0.0";

            Ready();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new AdminPortClientActor(sp));

        public void Ready()
        {
            StartWith(MainState.Idle, new IdleData());
            IdleState();
            ConnectingState();
            ConnectedState();

            WhenUnhandled(state =>
            {
                if(state.FsmEvent is MainActorSubscribe)
                {
                    this.Subscribers.Add(Sender);
                }
                else if(state.FsmEvent is MainActorDesubscribe)
                {
                    Subscribers.Remove(Sender);
                }
                return Stay();
            });
        }

        protected override void PostStop()
        {

            base.PostStop();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            switch (StateName)
            {
                case MainState.Connected:
                    return ConnectedSupervisorStrategy();
                default:
                    return base.SupervisorStrategy();

            }
        }

    }

}
