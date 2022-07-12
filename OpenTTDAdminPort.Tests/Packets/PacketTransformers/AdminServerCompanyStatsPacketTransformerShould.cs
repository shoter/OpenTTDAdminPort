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
    public class AdminServerCompanyStatsPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_STATS);
            packet.SendByte(2);

            packet.SendU16(1);
            packet.SendU16(2);
            packet.SendU16(3);
            packet.SendU16(4);
            packet.SendU16(5);

            packet.SendU16(11);
            packet.SendU16(12);
            packet.SendU16(13);
            packet.SendU16(14);
            packet.SendU16(15);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerCompanyStatsPacketTransformer()
                .Transform(packet)
                as AdminServerCompanyStatsMessage;

            Assert.Equal(1, msg.CompanyStats[2].VehicleCount[0]);
            Assert.Equal(2, msg.CompanyStats[2].VehicleCount[1]);
            Assert.Equal(3, msg.CompanyStats[2].VehicleCount[2]);
            Assert.Equal(4, msg.CompanyStats[2].VehicleCount[3]);
            Assert.Equal(5, msg.CompanyStats[2].VehicleCount[4]);

            Assert.Equal(11, msg.CompanyStats[2].StationCount[0]);
            Assert.Equal(12, msg.CompanyStats[2].StationCount[1]);
            Assert.Equal(13, msg.CompanyStats[2].StationCount[2]);
            Assert.Equal(14, msg.CompanyStats[2].StationCount[3]);
            Assert.Equal(15, msg.CompanyStats[2].StationCount[4]);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_STATS, new AdminServerCompanyStatsPacketTransformer().SupportedMessageType);
    }
}
