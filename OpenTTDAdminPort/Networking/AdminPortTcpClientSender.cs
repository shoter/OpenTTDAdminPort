using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClientSender : IAdminPortTcpClientSender
    {
        public WorkState State { get; private set; } = WorkState.NotStarted;

        public event EventHandler<Exception>? ErrorOcurred;

        private readonly ConcurrentQueue<IAdminMessage> messagesToSend = new ConcurrentQueue<IAdminMessage>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly IAdminPacketService adminPacketService;

        public AdminPortTcpClientSender(IAdminPacketService adminPacketService)
        {
            this.adminPacketService = adminPacketService;
        }

        public void SendMessage(IAdminMessage message)
        {
            messagesToSend.Enqueue(message);
        }

        public Task Start(Stream stream)
        {
            if (State != WorkState.NotStarted)
            {
                State = WorkState.Errored;
                cancellationTokenSource.Cancel();
                throw new AdminPortException("This Sender had been started before! You cannot start sender more than 1 time");
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(stream, cancellationTokenSource.Token)), null);
            State = WorkState.Working;
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            if (State == WorkState.Working)
            {
                cancellationTokenSource.Cancel();

                // give it some time to cancel MainLoop
                await Task.Delay(TimeSpan.FromSeconds(2));
                State = WorkState.Stopped;
            }
        }

        private async void MainLoop(Stream stream, CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.1));

                    while (messagesToSend.TryDequeue(out IAdminMessage msg))
                    {
                        Packet packet = this.adminPacketService.CreatePacket(msg);
                        await stream.WriteAsync(packet.Buffer, 0, packet.Size, token).WaitMax(TimeSpan.FromSeconds(2));
                    }
                }
                catch (Exception e)
                {
                    cancellationTokenSource.Cancel();
                    ErrorOcurred?.Invoke(this, e);
                    State = WorkState.Errored;
                }
            }

        }
    }
}
