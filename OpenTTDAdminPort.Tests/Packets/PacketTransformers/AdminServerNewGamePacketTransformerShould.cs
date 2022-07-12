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
    public class AdminServerNewGamePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_NEWGAME);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerNewGamePacketTransformer()
                .Transform(packet)
                as AdminServerNewGameMessage;

            Assert.NotNull(msg);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_NEWGAME, new AdminServerNewGamePacketTransformer().SupportedMessageType);
    }
}
