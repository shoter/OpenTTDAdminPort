﻿using System;
using System.IO;

using Akka.Actor;

using OpenTTDAdminPort.MainActor;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Watchdog;

namespace OpenTTDAdminPort.Akkas
{
    public class ActorFactory : IActorFactory
    {
        protected readonly IServiceProvider serviceProvider;

        private int n = 0;

        public ActorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public virtual IActorRef CreateActor(IActorContext context, Func<IServiceProvider, Props> propsCreator, string? name = null)
        {
            Props props = propsCreator.Invoke(serviceProvider);
            return context.ActorOf(props, name);
        }

        public virtual IActorRef CreateActor(IActorContext context, Func<Props> propsCreator)
        {
            Props props = propsCreator();
            return context.ActorOf(props);
        }

        public IActorRef CreateMainActor(ActorSystem actorSystem)
            => actorSystem.ActorOf(AdminPortClientActor.Create(this.serviceProvider), "Main");

        public IActorRef CreateMessager(IActorContext context)
            => CreateActor(context, AdminPortClientMessager.Create);

        public virtual IActorRef CreateReceiver(IActorContext context, Stream stream)
            => CreateActor(context, sp => AdminPortTcpClientReceiver.Create(sp, stream), $"Receiver{n++}");

        public virtual IActorRef CreateTcpClient(IActorContext context, string ip, int port)
            => CreateActor(context, sp => AdminPortTcpClient.Create(sp, ip, port), $"tcp{n++}");

        public virtual IActorRef CreateWatchdog(IActorContext context, IActorRef tcpClient, TimeSpan maximumPingTime)
            => CreateActor(context, sp => ConnectionWatchdog.Create(sp, tcpClient, maximumPingTime), $"watchdog{n++}");
    }
}
