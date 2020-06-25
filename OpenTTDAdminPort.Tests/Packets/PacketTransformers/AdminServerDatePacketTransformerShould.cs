using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.PacketTransformers
{
    public class AdminServerDatePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_DATE);
            packet.SendU32(365 /* year 1 */ + 31 /* February */ + 3); // date
            packet.PrepareToSend(); packet.ReadByte();

            var msg = new AdminServerDatePacketTransformer()
                .Transform(packet)
                as AdminServerDateMessage;

            Assert.Equal(1u, msg.Date.Year);
            Assert.Equal(1, msg.Date.Month);
            Assert.Equal(3, msg.Date.Day);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_DATE, new AdminServerDatePacketTransformer().SupportedMessageType);
    }
}
