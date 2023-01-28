using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            mainActor.Ask((Action<object>)OnMainActorMessage);
        }

        public async Task Connect(ILogger? test = null)
        {
            Console.WriteLine($"Trace = {test.IsEnabled(LogLevel.Trace)}");
            logger.LogTrace($"Asking MainActor {mainActor} to connect to server");
            await mainActor.TryAsk(new AdminPortConnect(ServerInfo, "AdminPortClient"));
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

        public void OnMainActorMessage(object msg)
        {
            switch (msg)
            {
                case IAdminEvent ev:
                    {
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
    }
}
