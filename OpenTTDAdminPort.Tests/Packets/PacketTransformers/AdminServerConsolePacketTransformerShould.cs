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
    public class AdminServerConsolePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE);

            packet.SendString("Origin");
            packet.SendString("String");

            packet.PrepareToSend();
            packet.ReadByte();

            var msg = new AdminServerConsolePacketTransformer()
                .Transform(packet)
                as AdminServerConsoleMessage;

            Assert.Equal("Origin", msg.Origin);
            Assert.Equal("String", msg.Message);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE, new AdminServerConsolePacketTransformer().SupportedMessageType);
    }
}
