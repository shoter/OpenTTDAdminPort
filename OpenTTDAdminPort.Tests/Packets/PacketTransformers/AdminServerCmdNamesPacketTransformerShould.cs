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
    public class AdminServerCmdNamesPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES);
            packet.SendByte(1);
            packet.SendU16(10);
            packet.SendString("First");
            packet.SendByte(1);
            packet.SendU16(20);
            packet.SendString("Sec");
            packet.SendByte(0);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCmdNamesPacketTransformer()
                .Transform(packet)
                as AdminServerCmdNamesMessage;

            Assert.Equal(2, msg.Commands.Count);
            Assert.Equal("First", msg.Commands[10]);
            Assert.Equal("Sec", msg.Commands[20]);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES, new AdminServerCmdNamesPacketTransformer().SupportedMessageType);

    }
}
