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
    public class AdminPingMessageTransformerShould
    {
        [Fact]
        public void TransformMessage_IfMessageIsCorrect()
        {
            IAdminMessage msg = new AdminPingMessage(argument: 43);
            Packet packet = new AdminPingMessageTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_PING, (AdminMessageType)packet.ReadByte());
            Assert.Equal(43u, packet.ReadU32());
        }
    }
}
