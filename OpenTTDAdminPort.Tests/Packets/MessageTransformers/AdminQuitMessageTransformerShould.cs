using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.MessageTransformers
{
    public class AdminQuitMessageTransformerShould
    {
        [Fact]
        public void TransformMessageIntoPacket()
        {
            IAdminMessage msg = new AdminQuitMessage();
            Packet packet = new AdminQuitMessageTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_QUIT, (AdminMessageType)packet.ReadByte());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_QUIT, new AdminQuitMessageTransformer().SupportedMessageType);
        }
    }
}
