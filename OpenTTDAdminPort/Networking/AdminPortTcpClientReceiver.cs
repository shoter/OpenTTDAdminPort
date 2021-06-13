﻿using Microsoft.Extensions.Logging;
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

        private bool isStopped = false;

        public WorkState State { get; private set; } = WorkState.NotStarted;

        public AdminPortTcpClientReceiver(IAdminPacketService adminPacketService, ILogger? logger = null)
        {
            this.adminPacketService = adminPacketService;
            this.logger = logger;
        }

        public Task Start(Stream stream)
        {
            this.cancellationTokenSource = new CancellationTokenSource();

            if (State != WorkState.NotStarted)
            {
                State = WorkState.Errored;
                cancellationTokenSource.Cancel();
                throw new AdminPortException("This Receiver had been started before! You cannot start receiver more than 1 time");
            }

            logger?.LogTrace("Receiver Starting!");

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(stream, cancellationTokenSource.Token)), null);
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => EventLoop(cancellationTokenSource.Token)), null);

            isStopped = false;
            State = WorkState.Working;

            logger?.LogTrace("Receiver Started!");


            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            if (State == WorkState.Working)
            {
                logger?.LogTrace("Receiver Stopping!");

                cancellationTokenSource.Cancel();

                if (!await TaskHelper.WaitUntil(() => isStopped, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)))
                {
                    throw new AdminPortException("Receiver waiting for Main Loop stop timed out");
                }

                State = WorkState.Stopped;
                logger?.LogTrace("Receiver Stopped!");
            }
        }

        private async void MainLoop(Stream stream, CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                try
                {
                    Packet packet = await WaitForPacket(stream, token);
                    IAdminMessage message = adminPacketService.ReadPacket(packet);
                    logger?.LogTrace($"Receiver received message {message}!");

                    if (!token.IsCancellationRequested)
                        receivedMessages.Enqueue(message);
                }
                catch (Exception e)
                {
                    cancellationTokenSource.Cancel();
                    ErrorOcurred?.Invoke(this, e);
                    State = WorkState.Errored;
                }
            }
            logger?.LogTrace("Receiver Main Loop Stopped!");

            isStopped = true;
        }

        private async Task<Packet> WaitForPacket(Stream stream, CancellationToken token)
        {
            byte[] sizeBuffer = await Read(stream, 2, token);
            ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
            byte[] content = await Read(stream, size - 2, token).WaitMax(TimeSpan.FromSeconds(5));
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
            } while (currentSize < dataSize);

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

                await Task.Delay(TimeSpan.FromSeconds(0.25));
            }
        }


    }
}
