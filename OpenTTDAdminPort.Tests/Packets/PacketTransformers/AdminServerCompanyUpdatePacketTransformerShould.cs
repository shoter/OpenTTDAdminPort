using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.PacketTransformers
{
    public class AdminServerCompanyUpdatePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE);
            packet.SendByte(11); // company id
            packet.SendString("Company Name");
            packet.SendString("Manager Name");
            packet.SendByte(5); // color
            packet.SendByte(1); // has password
            packet.SendByte(3); // months of bankruptcy

            // shares
            packet.SendByte(9);
            packet.SendByte(8);
            packet.SendByte(7);
            packet.SendByte(6);

            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCompanyUpdatePacketTransformer()
                .Transform(packet)
                as AdminServerCompanyUpdateMessage;

            Assert.Equal(11, msg.CompanyId);
            Assert.Equal("Company Name", msg.CompanyName);
            Assert.Equal("Manager Name", msg.ManagerName);
            Assert.Equal(5, msg.Color);
            Assert.True(msg.HasPassword);
            Assert.Equal(3, msg.MonthsOfBankruptcy);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE, new AdminServerCompanyUpdatePacketTransformer().SupportedMessageType);
    }
}
