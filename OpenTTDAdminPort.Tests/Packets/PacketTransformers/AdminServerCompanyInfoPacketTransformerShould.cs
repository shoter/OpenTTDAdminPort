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
    public class AdminServerCompanyInfoPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_INFO);
            packet.SendByte(11); // company id
            packet.SendString("Company Name");
            packet.SendString("Manager Name");
            packet.SendByte(1); // color
            packet.SendByte(0); // has password
            packet.SendU32(123); // TODO: Check for that later
            packet.SendByte(1); // is AI
            packet.SendByte(55); // months of bankruptcy

            packet.SendByte(9);
            packet.SendByte(8);
            packet.SendByte(7);
            packet.SendByte(6);

            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCompanyInfoPacketTransformer()
                .Transform(packet)
                as AdminServerCompanyInfoMessage;

            Assert.Equal(11, msg.CompanyId);
            Assert.Equal("Company Name", msg.CompanyName);
            Assert.Equal("Manager Name", msg.ManagerName);
            Assert.Equal(1, msg.Color);
            Assert.False(msg.HasPassword);
            Assert.True(msg.IsAi);
            Assert.Equal(55, msg.MonthsOfBankruptcy);
            Assert.Equal(9, msg.ShareOwnersIds[0]);
            Assert.Equal(8, msg.ShareOwnersIds[1]);
            Assert.Equal(7, msg.ShareOwnersIds[2]);
            Assert.Equal(6, msg.ShareOwnersIds[3]);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_INFO, new AdminServerCompanyInfoPacketTransformer().SupportedMessageType);
    }
}
