using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.PacketTransformers
{
    public class AdminServerCompanyRemovePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE);
            packet.SendByte(11); // company id
            packet.SendByte((byte)AdminCompanyRemoveReason.ADMIN_CRR_AUTOCLEAN);

            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCompanyRemovePacketTransformer()
                .Transform(packet)
                as AdminServerCompanyRemoveMessage;

            Assert.Equal(11, msg.CompanyId);
            Assert.Equal(AdminCompanyRemoveReason.ADMIN_CRR_AUTOCLEAN, msg.RemoveReason);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE, new AdminServerCompanyRemovePacketTransformer().SupportedMessageType);
    }
}
