using OpenTTDAdminPort.Game;
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
    public class AdminServerProtocolPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL);
            packet.SendByte(10); // version

            packet.SendByte(1);
            packet.SendU16((ushort)AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO);
            packet.SendU16((ushort)UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC);

            packet.SendByte(1);
            packet.SendU16((ushort)AdminUpdateType.ADMIN_UPDATE_CMD_NAMES);
            packet.SendU16((ushort)UpdateFrequency.ADMIN_FREQUENCY_POLL);

            packet.SendByte(0);
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerProtocolPacketTransformer()
                .Transform(packet)
                as AdminServerProtocolMessage;

            Assert.Equal(10, msg.NetworkVersion);
            Assert.Equal(UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC, msg.AdminUpdateSettings[AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO]);
            Assert.Equal(UpdateFrequency.ADMIN_FREQUENCY_POLL, msg.AdminUpdateSettings[AdminUpdateType.ADMIN_UPDATE_CMD_NAMES]);
        }
    }
}
