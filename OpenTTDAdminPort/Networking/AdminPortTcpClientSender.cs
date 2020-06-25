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

        private readonly Stream stream;
        private readonly IAdminPacketService adminPacketService;

        public AdminPortTcpClientSender(IAdminPacketService adminPacketService, Stream stream)
        {
            this.stream = stream;
            this.adminPacketService = adminPacketService;
        }

        public void SendMessage(IAdminMessage message)
        {
            messagesToSend.Enqueue(message);
        }

        public Task Start()
        {
            if (State != WorkState.NotStarted)
                throw new AdminPortException("This Sender had been started before! You cannot start sender more than 1 time");

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token)), null);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (State == WorkState.Working)
            {
                cancellationTokenSource.Cancel();
                State = WorkState.Stopped;
            }
            return Task.CompletedTask;
        }

        private async void MainLoop(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                try
                {
                    while (messagesToSend.TryDequeue(out IAdminMessage msg))
                    {
                        Packet packet = this.adminPacketService.CreatePacket(msg);
                        await stream.WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));
                    }

                    await Task.Delay(TimeSpan.FromSeconds(0.1));

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
