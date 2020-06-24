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
    public class AdminServerClientJoinPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN);
            packet.SendU32(11); //client id;
            packet.PrepareToSend();
            packet.ReadByte();

            var message = new AdminServerClientJoinPacketTransformer()
                .Transform(packet)
                as AdminServerClientJoinMessage;

            Assert.Equal(11u, message.ClientId);
        }
    }
}
