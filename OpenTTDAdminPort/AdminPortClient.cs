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

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class AdminPortClient : IAdminPortClient
    {
        public AdminConnectionState ConnectionState { get; private set; } = AdminConnectionState.Idle;

        /// <remarks>
        /// Null value until client connects to the server
        /// </remarks>
        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>? AdminUpdateSettings { get; set; }

        public ServerInfo ServerInfo { get; private set; }

        private IServiceProvider serviceProvider;

        private ActorSystem actorSystem;

        private IActorRef mainActor;

        //public event EventHandler<IAdminEvent>? EventReceived;

        public AdminPortClient(AdminPortClientSettings settings, ServerInfo serverInfo, Action<ILoggingBuilder>? configureLogging = null)
        {
            this.ServerInfo = serverInfo;
            this.actorSystem = ActorSystem.Create("AdminPortClient");

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IActorFactory, ActorFactory>();
            services.AddTransient<ITcpClient, MyTcpClient>();
            services.AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create());
            services.AddSingleton<IAdminEventFactory, AdminEventFactory>();
            services.AddSingleton(settings);
            services.AddLogging(configureLogging ?? delegate { });


            serviceProvider = services.BuildServiceProvider();


            IActorFactory actorFactory = serviceProvider.GetRequiredService<IActorFactory>();
            mainActor = actorFactory.CreateMainActor(actorSystem);
        }

        public async Task Connect()
        {
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
            mainActor.Ask(action);
        }
    }
}
