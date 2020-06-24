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
    public class AdminServerClientInfoPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO);
            packet.SendU32(123u); // client id
            packet.SendString("Hostname");
            packet.SendString("ClientName");
            packet.SendByte(11); // lang id
            packet.SendU32(5); // date
            packet.SendByte(1); // playing as
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerClientInfoPacketTransformer()
                .Transform(packet)
                as AdminServerClientInfoMessage;

            Assert.Equal(123u, msg.ClientId);
            Assert.Equal("Hostname", msg.Hostname);
            Assert.Equal("ClientName", msg.ClientName);
            Assert.Equal(11, msg.Language);
            Assert.Equal(6, msg.JoinDate.Day);
            Assert.Equal(0u, msg.JoinDate.Year);
            Assert.Equal(0, msg.JoinDate.Month);
            Assert.Equal(1, msg.PlayingAs);
        }
    }
}
