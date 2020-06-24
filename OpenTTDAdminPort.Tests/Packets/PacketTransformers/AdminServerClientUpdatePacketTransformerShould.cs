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
    public class AdminServerClientUpdatePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE);
            packet.SendU32(123);
            packet.SendString("STR");
            packet.SendByte(1);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerClientUpdatePacketTransformer()
                .Transform(packet)
                as AdminServerClientUpdateMessage;

            Assert.Equal(123u, msg.ClientId);
            Assert.Equal("STR", msg.ClientName);
            Assert.Equal(1, msg.PlayingAs);
        }
        
    }
}
