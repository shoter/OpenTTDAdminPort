using System;
using System.Collections.Generic;
using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Exceptions;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash, IWithTimers
    {
        private readonly IActorFactory actorFactory;

        private readonly Dictionary<Guid, IActorRef> waiterActors = new();

        private readonly string version;

        private readonly IServiceScope scope;

        private readonly ILogger logger;

        // Initialized by Akka.net
        public IStash Stash { get; set; } = default!;

        private IActorRef Messager { get; }

        private readonly IAdminEventFactory adminEventFactory;

        private IIncomingMessageProcessor incomingMessageProcessor = new IncomingMessageProcessor();

        // initialized by Akka.Net
        public ITimerScheduler Timers { get; set; } = default!;

        public AdminPortClientActor(IServiceProvider sp)
        {
            this.scope = sp.CreateScope();
            sp = this.scope.ServiceProvider;

            this.logger = sp.GetRequiredService<ILogger<AdminPortClientActor>>();
            this.actorFactory = sp.GetRequiredService<IActorFactory>();
            this.adminEventFactory = sp.GetRequiredService<IAdminEventFactory>();

            this.Messager = this.actorFactory.CreateMessager(Context);

            this.version = "1.0.0";

            this.logger.LogTrace($"AdminPortClientActor v {version} created");

            Ready();
        }

        public override void AroundPreRestart(Exception cause, object message)
        {
            if (cause != null && Sender != null)
            {
                Sender.Tell(cause);
            }

            base.AroundPreRestart(cause, message);
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new AdminPortClientActor(sp));

        public void Ready()
        {
            StartWith(MainState.Idle, new IdleData());
            IdleState();
            ConnectingState();
            ConnectedState();
            ConnectingSecureState();

            WhenUnhandled(state =>
            {
                if (state.FsmEvent is Action<object> action)
                {
                    logger.LogTrace($"Setting messager on receive event action");
                    this.Messager.Tell(action);
                }

                if (state.FsmEvent is WaitForEvent wfe)
                {
                    var actor = Context.ActorOf(AdminEventWaiterActor.Create(wfe.WaiterFunc, Sender));
                    Timers.StartSingleTimer(actor, new KillDanglingWaiter(wfe.RequestId, wfe.Token), TimeSpan.FromSeconds(30));
                    waiterActors.Add(wfe.RequestId, actor);
                }

                if (state.FsmEvent is KillDanglingWaiter kdw)
                {
                    if (!waiterActors.ContainsKey(kdw.WaiterId))
                    {
                        return Stay();
                    }

                    // Do not display this message when cancellation was requested
                    if (!kdw.Token.IsCancellationRequested)
                    {
                        this.logger.LogWarning($"Killing some dangling waiter. That should not happen buddy");
                    }

                    KillWaiter(kdw.WaiterId);
                }

                if (state.FsmEvent is KillWaiter kw)
                {
                    if (!waiterActors.ContainsKey(kw.RequestId))
                    {
                        return Stay();
                    }

                    KillWaiter(kw.RequestId);
                }

                switch(state.FsmEvent)
                {
                    case SendMessage:
                    case QueryServerStatus:
                        {
                            Stash.Stash();
                            break;
                        }

                    case QueryServerState:
                        {
                            var data = state.StateData;
                            Sender.Tell(data);
                            break;
                        }
                }

                return Stay();
            });
        }

        private void KillWaiter(Guid waiterId)
        {
            waiterActors[waiterId]
                .GracefulStop(3.Seconds());
            waiterActors.Remove(waiterId);
        }

        protected override void PostStop()
        {
            scope.Dispose();
            base.PostStop();
        }

        protected override SupervisorStrategy SupervisorStrategy()
            => new OneForOneStrategy(ex =>
            {
                logger.LogTrace($"SupervisorStrategy fired for {ex.GetType().Name}");
                if (ex.TryGetInnerException(typeof(InitialConnectionException), out var _))
                {
                    logger.LogTrace("Detected fatal tcp client exception. Sending message about this to myself in 3 seconds");
                    Timers.StartSingleTimer("fataltcp", new FatalTcpClientException(), 3.Seconds());
                    return Directive.Stop;
                }

                return Directive.Restart;
            });
    }
}