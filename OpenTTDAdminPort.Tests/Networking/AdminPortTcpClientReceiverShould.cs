using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientReceiverShould
    {
        IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
        [Fact]
        public async Task ReadPacketProperlyAndSendMessageAboutIt()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await WaitForMessage(receivedMessage);
            VerifyMessage(receivedMessage);
        }

        [Fact]
        public async Task BeAbleToDealWithSizeThatIsNotSendAsWholeAtOneMoment()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            await Task.Delay(100);
            stream.Write(packet.Buffer, 0, 1);
            await Task.Delay(100);
            stream.Write(packet.Buffer, 1, 1);
            await Task.Delay(100);
            stream.Write(packet.Buffer, 2, packet.Size - 2);
            stream.Position = 0;

            await WaitForMessage(receivedMessage);
            VerifyMessage(receivedMessage);
        }

        [Fact]
        public async Task BeAbletToDealWithDataThatIsNotSendAsWholeAtOneMoment()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            for(int i = 0; i < packet.Size; ++i)
            {
                stream.Write(packet.Buffer, i, 1);
                await Task.Delay(2);
            }

            stream.Position = 0;
            await WaitForMessage(receivedMessage);
            VerifyMessage(receivedMessage);
        }

        [Fact]
        public async Task BeAbleToDealWithLongWaitingTimeForPacket()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            await Task.Delay(TimeSpan.FromSeconds(10));

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await WaitForMessage(receivedMessage);
            VerifyMessage(receivedMessage);
        }

        [Fact]
        public async Task NotReceiveMessages_AfterStop()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            await receiver.Stop();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await WaitForMessage(receivedMessage);
            Assert.Null(receivedMessage);
        }

        [Fact]
        public async Task ChangeStatusToWorking_AfterStart()
        {
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();

            Assert.Equal(WorkState.Working, receiver.State);
        }

        [Fact]
        public async Task ThrowException_WhenStartingReceiverTwice()
        {
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            await Assert.ThrowsAsync<AdminPortException>(async () => await receiver.Start());

            Assert.Equal(WorkState.Errored, receiver.State);
        }

        [Fact]
        public async Task StopEverything_AfterSecondStart()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            try
            {
                await receiver.Start();
            }
            catch (Exception) { }
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await WaitForMessage(receivedMessage);
            Assert.Null(receivedMessage);
        }

        [Fact]
        public async Task ErrorOut_AfterSendingWrongPacket()
        {
            Packet packet = CreateWrongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(WorkState.Errored, receiver.State);
        }

        [Fact]
        public async Task NotBeingAbleToReceiveCorrectPacket_AfterWrongPacket()
        {
            Packet packet = CreateWrongMessage();
            using MemoryStream stream = new MemoryStream(2000);

            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            IAdminMessage receivedMessage = null;
            receiver.MessageReceived += (_, msg) => receivedMessage = msg;

            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;
            long savedPos = stream.Position;
            await Task.Delay(1000);
            packet = CreatePongMessage();
            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = savedPos;

            await WaitForMessage(receivedMessage);
            Assert.Null(receivedMessage);
        }

        [Fact]
        public async Task NotErrorOut_WhenMessageReceivedThrowsException()
        {
            Packet packet = CreatePongMessage();
            using MemoryStream stream = new MemoryStream(2000);
            var receiver = new AdminPortTcpClientReceiver(packetService, stream);
            await receiver.Start();
            receiver.MessageReceived += (_, msg) => throw new Exception("Peek a boo!");
            stream.Write(packet.Buffer, 0, packet.Size);
            stream.Position = 0;

            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(WorkState.Working, receiver.State);
        }


        private static async Task WaitForMessage(IAdminMessage receivedMessage)
        {
            for (int i = 0; i < 100; ++i)
            {
                if (receivedMessage != null)
                    break;
                await Task.Delay(1);
            }
        }



        private static void VerifyMessage(IAdminMessage receivedMessage)
        {
            var pongMsg = receivedMessage as AdminServerPongMessage;
            Assert.NotNull(pongMsg);
            Assert.Equal(123u, pongMsg.Argument);
        }

        private static Packet CreatePongMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_PONG);
            packet.SendU32(123u);
            packet.PrepareToSend();
            return packet;
        }

        private static Packet CreateWrongMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_ADMIN_CHAT);
            packet.PrepareToSend();
            return packet;
        }
    }
}
