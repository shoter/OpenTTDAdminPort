using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ConcurrentDictionary<uint, Player> Players { get; } = new ConcurrentDictionary<uint, Player>();

        /// <remarks>
        /// Null value until client connects to the server
        /// </remarks>
        public AdminServerInfo? AdminServerInfo { get ; private set; }

        /// <remarks>
        /// Null value until client connects to the server
        /// </remarks>
        public ServerInfo ServerInfo { get; private set; }

        private IServiceProvider serviceProvider;

        private ActorSystem actorSystem;

        private IActorRef mainActor;

        public event EventHandler<IAdminEvent>? EventReceived;

        public AdminPortClient(AdminPortClientSettings settings, ServerInfo serverInfo)
        {
            this.ServerInfo = serverInfo;
            this.actorSystem = ActorSystem.Create("AdminPortClient");

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IActorFactory, ActorFactory>();
            services.AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create());
            services.AddSingleton(settings);

            serviceProvider = services.BuildServiceProvider();


            IActorFactory actorFactory = serviceProvider.GetRequiredService<IActorFactory>();
            mainActor = actorFactory.CreateMainActor(actorSystem, AdminPortClientActor.Create);
        }

        public Task Connect()
        {
            mainActor.Ask(new AdminPortConnect(ServerInfo, "AdminPortClient"));
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            mainActor.Ask(new AdminPortDisconnect());
            return Task.CompletedTask;
        }

        public void SendMessage(IAdminMessage message)
        {
            mainActor.Ask(new SendMessage(message));
        }
    }
}
