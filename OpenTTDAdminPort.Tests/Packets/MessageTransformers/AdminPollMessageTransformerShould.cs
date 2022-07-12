using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.MessageTransformers
{
    public class AdminPollMessageTransformerShould
    {
        [Fact]
        public void TransformMessageIntoPacket()
        {
            IAdminMessage msg = new AdminPollMessage(Game.AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, 55u);
            Packet packet = new AdminPollMessageTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_POLL, (AdminMessageType)packet.ReadByte());
            Assert.Equal(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, (AdminUpdateType)packet.ReadByte());
            Assert.Equal(55u, packet.ReadU32());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_POLL, new AdminPollMessageTransformer().SupportedMessageType);
        }
    }
}
