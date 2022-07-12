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
    public class AdminGamescriptMessageTransformerShould
    {
        [Fact]
        public void ProperlyTransformMessage_ForCorrectMessage()
        {
            IAdminMessage msg = new AdminGamescriptMessage("{ json : 5 }");
            Packet packet = new AdminGamescriptMessageTransformer().Transform(msg);

            packet.PrepareToSend();
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_GAMESCRIPT, (AdminMessageType)packet.ReadByte());
            Assert.Equal("{ json : 5 }", packet.ReadString());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_GAMESCRIPT, new AdminGamescriptMessageTransformer().SupportedMessageType);
        }
    }
}
