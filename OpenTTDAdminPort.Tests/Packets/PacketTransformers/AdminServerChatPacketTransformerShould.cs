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
    public class AdminServerChatPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CHAT);
            packet.SendByte((byte)NetworkAction.NETWORK_ACTION_CHAT_CLIENT);
            packet.SendByte((byte)ChatDestination.DESTTYPE_CLIENT);
            packet.SendU32(23); // client ID
            packet.SendString("That is a message");
            packet.SendI64(123); // Data
            packet.PrepareToSend();
            packet.ReadByte(); // the first byte will be read by something else.

            var msg = new AdminServerChatPacketTransformer().Transform(packet) as AdminServerChatMessage;

            Assert.Equal(NetworkAction.NETWORK_ACTION_CHAT_CLIENT, msg.NetworkAction);
            Assert.Equal(ChatDestination.DESTTYPE_CLIENT, msg.ChatDestination);
            Assert.Equal(23u, msg.ClientId);
            Assert.Equal("That is a message", msg.Message);
            Assert.Equal(123, msg.Data);
        }
    }
}
