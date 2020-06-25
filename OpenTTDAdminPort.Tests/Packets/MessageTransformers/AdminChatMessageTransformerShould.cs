using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.MessageTransformers
{
    public class AdminChatMessageTransformerShould
    {
        [Fact]
        public void TransformMessageIntoPacket_WhenReceivedProperPacket()
        {
            AdminChatMessage msg = new AdminChatMessage(Game.NetworkAction.NETWORK_ACTION_CHAT,
                Game.ChatDestination.DESTTYPE_BROADCAST,
                destination: 5,
                message: "Hello there");

            Packet packet = new AdminChatMessageTransformer().Transform(msg);

            packet.PrepareToSend();
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_CHAT, (AdminMessageType)packet.ReadByte());
            Assert.Equal(NetworkAction.NETWORK_ACTION_CHAT, (NetworkAction)packet.ReadByte());
            Assert.Equal(ChatDestination.DESTTYPE_BROADCAST, (ChatDestination)packet.ReadByte());
            Assert.Equal(5u, packet.ReadU32());
            Assert.Equal("Hello there", packet.ReadString());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_CHAT, new AdminChatMessageTransformer().SupportedMessageType);
        }
    }
}
