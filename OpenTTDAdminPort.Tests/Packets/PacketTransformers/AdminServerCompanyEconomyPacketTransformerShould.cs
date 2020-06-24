using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.PacketTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.PacketTransformers
{
    public class AdminServerCompanyEconomyPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_ECONOMY);
            packet.SendByte(2); // company id;
            packet.SendU64(1000); // money
            packet.SendU64(50); // loan
            packet.SendU64(100); // income
            packet.SendU16(5); // delivered cargo

            //1st quarter
            packet.SendU64(500);
            packet.SendU16(6);
            packet.SendU16(7);
            //2nd quarter
            packet.SendU64(600);
            packet.SendU16(16);
            packet.SendU16(17);

            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCompanyEconomyPacketTransformer()
                .Transform(packet)
                as AdminServerCompanyEconomyMessage;

            Assert.Equal(2, msg.CompanyId);
            Assert.Equal(1000u, msg.Money);
            Assert.Equal(50u, msg.CurrentLoan);
            Assert.Equal(100u, msg.Income);
            Assert.Equal(5, msg.DeliveredCargo);

            Assert.Equal(500u, msg.Quarters[0].CompanyValue);
            Assert.Equal(6, msg.Quarters[0].PerformanceHistory);
            Assert.Equal(7, msg.Quarters[0].DeliveredCargo);

            Assert.Equal(600u, msg.Quarters[1].CompanyValue);
            Assert.Equal(16, msg.Quarters[1].PerformanceHistory);
            Assert.Equal(17, msg.Quarters[1].DeliveredCargo);
        }
    }
}