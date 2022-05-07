using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClientReceiver : ReceiveActor
    {
        private CancellationTokenSource receiveLoopCTS = new();

        private readonly IAdminPacketService adminPacketService;
        private readonly ILogger logger;
        private readonly IServiceScope scope;

        private Stream stream;

        public AdminPortTcpClientReceiver(IServiceProvider serviceProvider, Stream stream)
        {
            scope = serviceProvider.CreateScope();
            serviceProvider = scope.ServiceProvider;
            this.adminPacketService = serviceProvider.GetRequiredService<IAdminPacketService>();
            this.logger = serviceProvider.GetRequiredService<ILogger<AdminPortTcpClientReceiver>>();
            this.stream = stream;

            var myself = Self;
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => ReceiveLoop(receiveLoopCTS.Token, myself)), null);
            Receive<ReceiveLoopException>(e =>
            {
                logger.LogError("I received receive loop exception. I am killing myself");
                throw e;
            });
            Receive<IAdminMessage>(m => Context.Parent.Tell(new ReceiveMessage(m)));
        }

        protected override void PreRestart(Exception reason, object message)
        {
            if(reason != null)
            {
                Sender.Tell(reason);
            }

            base.PreRestart(reason, message);
        }

        public static Props Create(IServiceProvider serviceProvider, Stream stream) => Props.Create(() => new AdminPortTcpClientReceiver(serviceProvider, stream));

        protected override void PostStop()
        {
            scope.Dispose();
            receiveLoopCTS.Cancel();
            base.PostStop();
        }

        private async void ReceiveLoop(CancellationToken token, IActorRef self)
        {
            try
            {
                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        Packet packet = await WaitForPacket(stream, token);
                        logger.LogTrace($"Receiver receiving new message!");
                        IAdminMessage message = adminPacketService.ReadPacket(packet);
                        logger.LogTrace($"Receiver received message {message}!");

                        if (!token.IsCancellationRequested)
                        {
                            self.Tell(message);
                        }
                    }
                    catch (Exception e) when (!(e is TaskCanceledException))
                    {
                        logger.LogError(e, e.ToString());
                        self.Tell(new ReceiveLoopException("Something went wrong in receive loop", e));
                    }
                    catch (Exception e) when (e is TaskCanceledException)
                    {
                        logger.LogInformation("Receiver loop receives TaskCancelled Exception");
                    }
                }
                logger?.LogTrace("Receiver Main Loop Stopped!");
            }
            catch (Exception e)
            {
                logger.LogError(e, e.ToString());
                self.Tell(new ReceiveLoopException("Something went wrong in receive loop", e));
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
                logger?.LogTrace($"{DateTime.Now:hh mm ss} Receiver trying to receive packet({token.IsCancellationRequested})");
            } while (currentSize < dataSize && !token.IsCancellationRequested);

            return result;
        }

    }
}
