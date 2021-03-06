﻿using OpenTTDAdminPort.Game;
using System.Collections.Generic;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerProtocolMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL;

        public byte NetworkVersion { get; }

        public Dictionary<AdminUpdateType, UpdateFrequency> AdminUpdateSettings;
        public AdminServerProtocolMessage(byte networkVersion, Dictionary<AdminUpdateType, UpdateFrequency> adminUpdateSettings)
        {
            this.NetworkVersion = networkVersion;
            this.AdminUpdateSettings = adminUpdateSettings;
        }
    }
}
