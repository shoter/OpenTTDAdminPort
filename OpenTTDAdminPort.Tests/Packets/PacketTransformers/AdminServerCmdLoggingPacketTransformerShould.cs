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
    public class AdminServerCmdLoggingPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CMD_LOGGING);
            packet.SendU32(11); //client id
            packet.SendByte(1); // company id
            packet.SendU16(321); // cmd
            packet.SendU32(5); // p1
            packet.SendU32(6); // p2
            packet.SendU32(99); // tile
            packet.SendString("Text");
            packet.SendU32(9999); // frame
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCmdLoggingPacketTransformer()
                .Transform(packet)
                as AdminServerCmdLoggingMessage;

            Assert.Equal(11u, msg.ClientId);
            Assert.Equal(1, msg.CompanyId);
            Assert.Equal(321, msg.Cmd);
            Assert.Equal(5u, msg.P1);
            Assert.Equal(6u, msg.P2);
            Assert.Equal(99u, msg.Tile);
            Assert.Equal("Text", msg.Text);
            Assert.Equal(9999u, msg.Frame);
        }
    }
}
