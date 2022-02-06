using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;

using System;
using System.IO;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClientSender : ReceiveActor
    {
        private readonly IAdminPacketService adminPacketService;
        private readonly ILogger logger;
        private readonly IServiceScope serviceScope;

        public AdminPortTcpClientSender(IServiceProvider serviceProvider, Stream tcpStream)
        {
            this.serviceScope = serviceProvider.CreateScope();
            serviceProvider = serviceScope.ServiceProvider;
            this.adminPacketService = serviceProvider.GetRequiredService<IAdminPacketService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<AdminPortTcpClientSender>>();

            ReceiveAsync<IAdminMessage>(async msg =>
            {
                try
                {
                    logger.LogTrace($"Sender sending {msg}!");

                    Packet packet = this.adminPacketService.CreatePacket(msg);
                    await tcpStream.WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));

                    logger.LogTrace($"Sender sent {msg}!");
                }
                catch (Exception e)
                {
                    logger?.LogError(e, "Sender errored");
                    throw new SendException("Message sending failed", e);
                }
            });
        }
    }
}
