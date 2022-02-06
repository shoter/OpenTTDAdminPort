using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;

using System;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClient : ReceiveActor
    {

        private void ConnectedReady()
        {
            ReceiveAsync<SendMessage>(async sendMessage =>
            {
                try
                {
                    IAdminMessage msg = sendMessage.Message;

                    logger.LogTrace($"Sender sending {msg}!");
                    Packet packet = this.adminPacketService.CreatePacket(msg);
                    await stream!.WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));
                    logger.LogTrace($"Sender sent {msg}!");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Sender errored");
                    throw new SendException("Message sending failed", e);
                }
            });

            Receive<AdminPortTcpClientDisconnect>(_ =>
            {
                receiver.Tell(PoisonPill.Instance);
                receiver = null;

                tcpClient.Dispose();
                stream = null;
                tcpClient = scope.ServiceProvider.GetRequiredService<ITcpClient>();

                UnbecomeStacked();
            });

            Receive<ReceiveMessage>(receiveMessage =>
            {
                Context.Parent.Tell(receiveMessage);
            });
        }

    }
}
