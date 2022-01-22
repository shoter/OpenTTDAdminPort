using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClientReceiver : IAdminPortTcpClientReceiver
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<Exception>? ErrorOcurred;
        public event EventHandler<IAdminMessage>? MessageReceived;

        private readonly ConcurrentQueue<IAdminMessage> receivedMessages = new ConcurrentQueue<IAdminMessage>();
        private readonly IAdminPacketService adminPacketService;
        private readonly ILogger? logger;

        private bool isStopped = true;

        public WorkState State { get; private set; } = WorkState.NotStarted;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);


        public AdminPortTcpClientReceiver(IAdminPacketService adminPacketService, ILogger? logger = null)
        {
            this.adminPacketService = adminPacketService;
            this.logger = logger;
        }

        public async Task Start(Stream stream)
        {
            await semaphore.WaitAsync();

            try
            {
                if (State != WorkState.NotStarted)
                {
                    State = WorkState.Errored;
                    cancellationTokenSource.Cancel();
                    throw new AdminPortException("This Receiver had been started before! You cannot start receiver more than 1 time");
                }

                State = WorkState.Working;

                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource = new CancellationTokenSource();


                logger?.LogTrace("Receiver Starting!");

                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(stream, cancellationTokenSource.Token)), null);
                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => EventLoop(cancellationTokenSource.Token)), null);

                isStopped = false;

                logger?.LogTrace("Receiver Started!");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task Stop()
        {
            await semaphore.WaitAsync();
            try
            {
                if (State == WorkState.Working)
                {
                    logger?.LogTrace("Receiver Stopping!");

                    cancellationTokenSource.Cancel();

                    if (!await TaskHelper.WaitUntil(() => isStopped, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(100)))
                    {
                        throw new AdminPortException("Receiver waiting for Main Loop stop timed out");
                    }

                    State = WorkState.Stopped;
                    logger?.LogTrace("Receiver Stopped!");
                }
            }
            finally
            {
                semaphore.Release();
            }

        }

        private async void MainLoop(Stream stream, CancellationToken token)
        {
            try
            {
                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        Packet packet = await WaitForPacket(stream, token);
                        logger?.LogTrace($"Receiver receiving new message!");
                        IAdminMessage message = adminPacketService.ReadPacket(packet);
                        logger?.LogTrace($"Receiver received message {message}!");

                        if (!token.IsCancellationRequested)
                            receivedMessages.Enqueue(message);
                    }
                    catch (Exception e) when (!(e is TaskCanceledException))
                    {
                        logger?.LogError(e, e.ToString());
                        cancellationTokenSource.Cancel();
                        ErrorOcurred?.Invoke(this, e);
                        State = WorkState.Errored;
                    }
                    catch (Exception e) when (e is TaskCanceledException)
                    {
                        logger?.LogInformation("Receiver main loop receives TaskCancelled Exception");

                    }
                }
                logger?.LogTrace("Receiver Main Loop Stopped!");
            }
            finally
            {
                isStopped = true;
            }
        }

        private async Task<Packet> WaitForPacket(Stream stream, CancellationToken token)
        {
            byte[] sizeBuffer = await Read(stream, 2, token);

            if (token.IsCancellationRequested)
            {
                // Task read has been probably interrupted. In this situation it is highly likely that we have gibberish in the array. It is better to not read it.
                throw new TaskCanceledException();
            }

            ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
            byte[] content = await Read(stream, size - 2, token).WaitMax(TimeSpan.FromSeconds(5));

            if (token.IsCancellationRequested)
            {
                // Task read has been probably interrupted. In this situation it is highly likely that we have gibberish in the array. It is better to not read it.
                throw new TaskCanceledException();
            }
            Packet packet = CreatePacket(sizeBuffer, content);
            return packet;
        }

        private static Packet CreatePacket(byte[] sizeBuffer, byte[] content)
        {
            byte[] packetData = new byte[2 + content.Length];
            packetData[0] = sizeBuffer[0];
            packetData[1] = sizeBuffer[1];
            for (int i = 0; i < content.Length; ++i)
                packetData[2 + i] = content[i];

            Packet packet = new Packet(packetData);
            return packet;
        }

        private async Task<byte[]> Read(Stream stream, int dataSize, CancellationToken token)
        {
            byte[] result = new byte[dataSize];
            int currentSize = 0;

            do
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                Task<int> task = stream
                    .ReadAsync(result, currentSize, dataSize - currentSize, token)
                    .WaitWithToken(token);
                await task;
                currentSize += task.Result;
                logger?.LogTrace($"Receiver trying to receive packet({token.IsCancellationRequested})");
            } while (currentSize < dataSize && !token.IsCancellationRequested);

            return result;
        }

        private async void EventLoop(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                if (receivedMessages.TryDequeue(out IAdminMessage msg))
                {
                    try
                    {
                        MessageReceived?.Invoke(this, msg);
                    }
                    catch (Exception e)
                    {
                        // TODO: log it?
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }

            logger?.LogTrace("Receiver Event Loop Stopped!");

        }


    }
}
