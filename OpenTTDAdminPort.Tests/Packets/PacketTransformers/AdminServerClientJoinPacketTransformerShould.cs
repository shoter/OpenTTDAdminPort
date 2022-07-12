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
    public class AdminServerClientJoinPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN);

            // client id;
            packet.SendU32(11);
            packet.PrepareToSend();
            packet.ReadByte();

            var message = new AdminServerClientJoinPacketTransformer()
                .Transform(packet)
                as AdminServerClientJoinMessage;

            Assert.Equal(11u, message.ClientId);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN, new AdminServerClientJoinPacketTransformer().SupportedMessageType);
    }
}
