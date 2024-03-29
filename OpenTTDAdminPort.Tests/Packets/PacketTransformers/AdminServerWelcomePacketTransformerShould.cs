﻿using System;
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
    public class AdminServerWelcomePacketTransformerShould
    {
        [Fact]
        public void TransformPacketIntoMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_WELCOME);
            packet.SendString("Server Name");
            packet.SendString("Revision");
            packet.SendByte(1); // dedicated
            packet.SendString("Map Name");
            packet.SendU32(1234); // seed
            packet.SendByte((byte)Landscape.LT_ARCTIC);
            packet.SendU32(5); // date
            packet.SendU16(20); // width
            packet.SendU16(40); // height

            packet.PrepareToSend();
            packet.ReadByte();

            IPacketTransformer<AdminServerWelcomeMessage> transformer = new AdminServerWelcomePacketTransformer();
            var msg = transformer.TransformTyped(packet);

            Assert.Equal("Server Name", msg.ServerName);
            Assert.Equal("Revision", msg.NetworkRevision);
            Assert.True(msg.IsDedicated);
            Assert.Equal("Map Name", msg.MapName);
            Assert.Equal(1234u, msg.MapSeed);
            Assert.Equal(Landscape.LT_ARCTIC, msg.Landscape);
            Assert.Equal(6, msg.CurrentDate.Day);
            Assert.Equal(20, msg.MapWidth);
            Assert.Equal(40, msg.MapHeight);
        }

        [Fact]
        public void HaveCorrectMessageType() => Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_WELCOME, new AdminServerWelcomePacketTransformer().SupportedMessageType);
    }
}
