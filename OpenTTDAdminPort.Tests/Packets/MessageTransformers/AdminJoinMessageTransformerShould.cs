using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.MessageTransformers
{
    public class AdminJoinMessageTransformerShould
    {
        [Fact]
        public void TransformMessageToPacket_ForGoodPacket()
        {
            IAdminMessage msg = new AdminJoinMessage("pass", "name", "ver");
            Packet packet = new AdminJoinMessageTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_JOIN, (AdminMessageType)packet.ReadByte());
            Assert.Equal("pass", packet.ReadString());
            Assert.Equal("name", packet.ReadString());
            Assert.Equal("ver", packet.ReadString());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_JOIN, new AdminJoinMessageTransformer().SupportedMessageType);
        }
    }
}
