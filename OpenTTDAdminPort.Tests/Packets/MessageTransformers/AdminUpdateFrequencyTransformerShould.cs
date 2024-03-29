﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;

using Xunit;

namespace OpenTTDAdminPort.Tests.Packets.MessageTransformers
{
    public class AdminUpdateFrequencyTransformerShould
    {
        [Fact]
        public void TransformMessageIntoPacket()
        {
            IAdminMessage msg = new AdminUpdateFrequencyMessage(
                AdminUpdateType.ADMIN_UPDATE_CMD_LOGGING,
                UpdateFrequency.ADMIN_FREQUENCY_DAILY);
            Packet packet = new AdminUpdateFrequencyTransformer().Transform(msg);
            packet.PrepareToSend();

            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY, (AdminMessageType)packet.ReadByte());
            Assert.Equal(AdminUpdateType.ADMIN_UPDATE_CMD_LOGGING, (AdminUpdateType)packet.ReadU16());
            Assert.Equal(UpdateFrequency.ADMIN_FREQUENCY_DAILY, (UpdateFrequency)packet.ReadU16());
        }

        [Fact]
        public void HaveCorrectMessageType()
        {
            Assert.Equal(AdminMessageType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY, new AdminUpdateFrequencyTransformer().SupportedMessageType);
        }
    }
}
