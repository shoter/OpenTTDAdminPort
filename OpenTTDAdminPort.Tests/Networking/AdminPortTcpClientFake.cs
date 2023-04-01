using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Exceptions;
using OpenTTDAdminPort.Packets;

namespace OpenTTDAdminPort.Tests.Networking
{
    internal class AdminPortTcpClientFake : ScopedReceiveActor
    {
        private readonly ILogger logger;
        private readonly FakeTcpData data = default!;

        public AdminPortTcpClientFake(IServiceProvider serviceProvider, string ip, int port)
            : base(serviceProvider)
        {
            this.logger = SP.GetRequiredService<ILogger<AdminPortTcpClientFake>>();

            Ready();
        }

        public static Props Create(IServiceProvider sp, string ip, int port)
            => Props.Create(() => new AdminPortTcpClient(sp, ip, port));

        private void Ready()
        {
            Receive<SendMessage>(SendMessage);
        }

        private void SendMessage(SendMessage sendMessage)
        {
            switch(sendMessage.Message)
            {
                case AdminJoinMessage joinMsg:
                    {
                        Parent.Tell(data.ProtocolMessage);
                        Parent.Tell(data.WelcomeMessage);
                        break;
                    }
            }
        }
    }
}
