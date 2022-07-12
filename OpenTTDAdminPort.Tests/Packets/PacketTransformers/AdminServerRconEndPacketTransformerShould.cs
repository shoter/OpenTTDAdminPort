using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.PacketTransformers
{
    public class AdminServerRconEndPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_RCON_END);
            packet.SendString("Command");
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerRconEndPacketTransformer()
                .Transform(packet)
                as AdminServerRconEndMessage;

            Assert.Equal("Command", msg.Command);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_RCON_END, new AdminServerRconEndPacketTransformer().SupportedMessageType);
    }
}
