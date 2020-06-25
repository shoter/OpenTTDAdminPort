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

            Packet packet = adminPacketService.CreatePacket(msg);

            stream.Position = 0;
            for (int i = 0; i < packet.Size; ++i)
            {
                Assert.Equal(packet.Buffer[i], stream.ReadByte());
            }
        }

    }
}
