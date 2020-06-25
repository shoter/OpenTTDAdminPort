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
    public class AdminRconMessageTransformerShould
    {
        [Fact]
        public void TransformMessageIntoPacket()
        {
            IAdminMessage msg = new AdminRconMessage("kick shoter");
            Packet packet = new AdminRconMessageTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_RCON, (AdminMessageType)packet.ReadByte());
            Assert.Equal("kick shoter", packet.ReadString());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_RCON, new AdminRconMessageTransformer().SupportedMessageType);
        }

    }
}
