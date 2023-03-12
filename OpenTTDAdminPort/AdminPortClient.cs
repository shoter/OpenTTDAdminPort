using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;

namespace OpenTTDAdminPort
{
    public class AdminPortClient : IAdminPortClient
    {
        public AdminConnectionState ConnectionState { get; private set; } = AdminConnectionState.Idle;

        /// <remarks>
        /// Null value until client connects to the server
        /// </remarks>
        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>? AdminUpdateSettings { get; set; }

        public ServerInfo ServerInfo { get; }

        private ActorSystem actorSystem;

        private IActorRef mainActor;

        private Action<IAdminEvent> onAdminEventReceive = _ => { };

        private Action<AdminConnectionStateChange> onConnectionStateChange = _ => { };

        private ILogger logger;

        private LoggerFactory? f;

        public AdminPortClient(AdminPortClientSettings settings, ServerInfo serverInfo, Action<ILoggingBuilder>? configureLogging = null)
        {
            this.ServerInfo = serverInfo;
            this.actorSystem = ActorSystem.Create("AdminPortClient");

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IActorFactory, ActorFactory>();
            services.AddTransient<ITcpClient, MyTcpClient>();
            services.AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create());
            services.AddSingleton<IAdminEventFactory, AdminEventFactory>();
            services.Configure<AdminPortClientSettings>(s =>
            {
                s.WatchdogInterval = settings.WatchdogInterval;
            });
            services.AddLogging(configureLogging ?? ((_) => { }));

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            this.logger = serviceProvider.GetRequiredService<ILogger<AdminPortClient>>();

            this.logger.LogTrace("Created service provider");
            IActorFactory actorFactory = serviceProvider.GetRequiredService<IActorFactory>();
            this.logger.LogTrace("Created actor factory");
            mainActor = actorFactory.CreateMainActor(actorSystem);
            this.logger.LogTrace("Created main actor");
            SetMainActorMessagerAction();

            f = serviceProvider.GetService<ILoggerFactory>() as LoggerFactory;
        }

        private void SetMainActorMessagerAction()
        {
            mainActor.Ask((Action<object>)OnMainActorMessage);
        }

        public async Task Connect(ILogger? test = null)
        {
            try
            {
                logger.LogTrace($"Asking MainActor {mainActor} to connect to server");
                await mainActor.TryAsk(new AdminPortConnect(ServerInfo, "AdminPortClient"));
                logger.LogTrace("Main actor connected!");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error during connect");
            }
        }

        public async Task Disconnect()
        {
            await mainActor.TryAsk(new AdminPortDisconnect());
        }

        public void SendMessage(IAdminMessage message)
        {
            mainActor.Ask(new SendMessage(message));
        }

        public void SetAdminEventHandler(Action<IAdminEvent> action)
        {
            onAdminEventReceive = action;
        }

        public void SetConnectionStateChangeHandler(Action<AdminConnectionStateChange> action)
        {
            onConnectionStateChange = action;
        }

        private void OnMainActorMessage(object msg)
        {
            switch (msg)
            {
                case IAdminEvent ev:
                    {
                        logger.LogTrace($"Received event {ev}");
                        onAdminEventReceive.Invoke(ev);
                        break;
                    }

                case AdminPortClientStateChange stateChange:
                    {
                        AdminConnectionState newState = stateChange.NewState.ToConnectionState();
                        AdminConnectionState previousState = this.ConnectionState;
                        this.ConnectionState = newState;
                        onConnectionStateChange(new AdminConnectionStateChange(previousState, newState));
                        break;
                    }
            }
        }

        public async Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, CancellationToken token = default)
            where TEvent : IAdminEvent
        {
            return (TEvent)await WaitForEvent(messageToSend, ev => ev is TEvent, TimeSpan.FromSeconds(30), token);
        }

        public async Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, Func<TEvent, bool> func, CancellationToken token = default)
            where TEvent : IAdminEvent
        {
            bool RealFunc(IAdminEvent ev)
            {
                if (ev is not TEvent)
                {
                    return false;
                }

                return func((TEvent)ev);
            }

            return (TEvent)await WaitForEvent(messageToSend, RealFunc, TimeSpan.FromSeconds(30), token);
        }

        public async Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, TimeSpan timeout, CancellationToken token = default)
            where TEvent : IAdminEvent
        {
            return (TEvent)await WaitForEvent(messageToSend, ev => ev is TEvent, timeout, token);
        }

        public async Task<IAdminEvent> WaitForEvent(IAdminMessage messageToSend, Func<IAdminEvent, bool> func, TimeSpan timeout, CancellationToken token = default)
        {
            Task<object> task = mainActor.Ask(new WaitForEvent(func));
            this.mainActor.Tell(new SendMessage(messageToSend));
            await Task.WhenAny(task, Task.Delay(timeout, token));

            if(!task.IsCompleted)
            {
                throw new TimeoutException();
            }

            return (IAdminEvent)task.Result;
        }

        public async Task<ServerStatus> QueryServerStatus(CancellationToken token = default)
        {
            return await mainActor.Ask<ServerStatus>(new QueryServerStatus());
        }
    }
}
