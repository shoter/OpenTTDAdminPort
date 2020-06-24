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
    public class AdminServerCompanyNewPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_NEW);
            packet.SendByte(11); // company id

            packet.PrepareToSend(); packet.ReadByte();

            var msg = new AdminServerCompanyNewPacketTransformer()
                .Transform(packet)
                as AdminServerCompanyNewMessage;

            Assert.Equal(11, msg.CompanyId);
        }
    }
}
