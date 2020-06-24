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
    public class AdminServerClientQuitPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT);
            packet.SendU32(123);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerClientQuitPacketTransformer()
                .Transform(packet)
                as AdminServerClientQuitMessage;

            Assert.Equal(123u, msg.ClientId);
        }
    }
}
