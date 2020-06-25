using OpenTTDAdminPort.Game;
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
    public class AdminServerClientErrorPacketTransformerShould
    {
        [Fact]
        public void ShouldTransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR);
            packet.SendU32(123); // client id
            packet.SendByte((byte)NetworkErrorCode.NETWORK_ERROR_CHEATER); // error
            packet.PrepareToSend();
            packet.ReadByte(); // the first byte will be read by something else.

            var msg = new AdminServerClientErrorPacketTransformer().Transform(packet) as AdminServerClientErrorMessage;
            Assert.Equal(123u, msg.ClientId);
            Assert.Equal(NetworkErrorCode.NETWORK_ERROR_CHEATER, msg.Error);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR, new AdminServerClientErrorPacketTransformer().SupportedMessageType);


    }
}
