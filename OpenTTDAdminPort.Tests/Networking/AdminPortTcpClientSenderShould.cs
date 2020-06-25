using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortTcpClientSenderShould
    {
        private MemoryStream stream = new MemoryStream();
        private AdminPortTcpClientSender sender;
        private IAdminPacketService adminPacketService = new AdminPacketServiceFactory().Create();


        public AdminPortTcpClientSenderShould()
        {
            sender = new AdminPortTcpClientSender(adminPacketService, stream);
            // We will always start sender.
            sender.Start().Wait();
        }

        [Fact]
        public async Task BeAbleToSendPacket()
        {
            AdminPingMessage msg = new AdminPingMessage(33u);
            sender.SendMessage(msg);

            await Task.Delay(TimeSpan.FromSeconds(1));

            Verify(msg);
        }



        [Fact]
        public async Task ThrowException_AfterSecondStart()
        {
            await Assert.ThrowsAsync<AdminPortException>(async () => await sender.Start());
            Assert.Equal(WorkState.Errored, sender.State);
        }

        [Fact]
        public async Task NotAcceptNewMessages_AfterSecondStart()
        {
            await Assert.ThrowsAsync<AdminPortException>(async () => await sender.Start());

            AdminPingMessage msg = new AdminPingMessage(33u);
            sender.SendMessage(msg);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(0, stream.Position);
        }


        [Fact]
        public async Task ErrorOut_WhenWrongMessageIsPassed()
        {
            var msg = new AdminServerShutdownMessage();
            sender.SendMessage(msg);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.Equal(WorkState.Errored, sender.State);
        }

        [Fact]
        public async Task SendException_WhenWrongMessageIsPassed()
        {
            Exception exception = null;
            sender.ErrorOcurred += (_, e) => exception = e;

            var msg = new AdminServerShutdownMessage();
            sender.SendMessage(msg);
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.NotNull(exception);
        }




        private void Verify(AdminPingMessage msg)
        {
            Packet packet = adminPacketService.CreatePacket(msg);

            stream.Position = 0;
            for (int i = 0; i < packet.Size; ++i)
            {
                Assert.Equal(packet.Buffer[i], stream.ReadByte());
            }
        }


    }
}
