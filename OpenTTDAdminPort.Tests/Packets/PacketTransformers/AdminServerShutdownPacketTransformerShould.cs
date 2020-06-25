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
    public class AdminServerShutdownPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_SHUTDOWN);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerShutdownPacketTransformer()
                .Transform(packet)
                as AdminServerShutdownMessage;

            Assert.NotNull(msg);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_SHUTDOWN, new AdminServerShutdownPacketTransformer().SupportedMessageType);
    }
}
