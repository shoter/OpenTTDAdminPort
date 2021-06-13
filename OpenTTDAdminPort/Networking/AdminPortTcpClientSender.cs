using Microsoft.Extensions.Logging;
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

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly IAdminPacketService adminPacketService;

        private bool isStopped = false;

        private readonly ILogger? logger;

        public AdminPortTcpClientSender(IAdminPacketService adminPacketService, ILogger? logger = null)
        {
            this.adminPacketService = adminPacketService;
            this.logger = logger;
        }

        public void SendMessage(IAdminMessage message)
        {
            messagesToSend.Enqueue(message);
        }

        public Task Start(Stream stream)
        {
            cancellationTokenSource = new CancellationTokenSource();
            if (State != WorkState.NotStarted)
            {
                State = WorkState.Errored;
                cancellationTokenSource.Cancel();
                throw new AdminPortException("This Sender had been started before! You cannot start sender more than 1 time");
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(stream, cancellationTokenSource.Token)), null);
            isStopped = false;
            State = WorkState.Working;

            logger?.LogTrace("Sender started!");
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            if (State == WorkState.Working)
            {
                logger?.LogTrace("Sender Stopping!");

                cancellationTokenSource.Cancel();

                if (!await TaskHelper.WaitUntil(() => isStopped, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)))
                {
                    throw new AdminPortException("Sender waiting for Main Loop stop timed out");
                }

                State = WorkState.Stopped;
                logger?.LogTrace("Sender Stopped!");

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
                        logger?.LogTrace($"Sender sending {msg}!");

                        Packet packet = this.adminPacketService.CreatePacket(msg);
                        await stream.WriteAsync(packet.Buffer, 0, packet.Size, token).WaitMax(TimeSpan.FromSeconds(2)).WaitWithToken(token);
                    }
                }
                catch (Exception e)
                {
                    logger?.LogError(e, "Sender errored");

                    cancellationTokenSource.Cancel();
                    ErrorOcurred?.Invoke(this, e);
                    State = WorkState.Errored;
                }
            }
            logger?.LogTrace("Sender Main Loop Stopped!");
            isStopped = true;

        }
    }
}
