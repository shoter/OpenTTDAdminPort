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
    public class AdminServerPongPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_PONG);
            packet.SendU32(123);
            packet.PrepareToSend(); packet.ReadByte();

            var msg = new AdminServerPongPacketTransformer()
                .Transform(packet)
                as AdminServerPongMessage;

            Assert.Equal(123u, msg.Argument);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_PONG, new AdminServerPongPacketTransformer().SupportedMessageType);
    }
}
