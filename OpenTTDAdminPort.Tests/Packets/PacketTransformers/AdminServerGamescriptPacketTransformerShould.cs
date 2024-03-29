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
    public class AdminServerGamescriptPacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_GAMESCRIPT);
            packet.SendString("{json:5}");
            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerGamescriptPacketTransformer()
                .Transform(packet)
                as AdminServerGamescriptMessage;

            Assert.Equal("{json:5}", msg.Json);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_GAMESCRIPT, new AdminServerGamescriptPacketTransformer().SupportedMessageType);
    }
}
