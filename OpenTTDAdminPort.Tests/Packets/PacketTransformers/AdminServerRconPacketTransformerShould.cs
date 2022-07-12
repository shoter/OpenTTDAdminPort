﻿using System;
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
    public class AdminServerRconPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_RCON);
            packet.SendU16(12); // color;
            packet.SendString("Result");
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerRconPacketTransformer()
                .Transform(packet)
                as AdminServerRconMessage;

            Assert.Equal(12, msg.Color);
            Assert.Equal("Result", msg.Result);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_RCON, new AdminServerRconPacketTransformer().SupportedMessageType);
    }
}
