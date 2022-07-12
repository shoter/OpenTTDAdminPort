﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    public class AdminServerChatMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CHAT;

        public NetworkAction NetworkAction { get; internal set; }

        public ChatDestination ChatDestination { get; internal set; }

        public uint ClientId { get; internal set; }

        public string Message { get; internal set; } = default!;

        public long Data { get; internal set; }
    }
}
