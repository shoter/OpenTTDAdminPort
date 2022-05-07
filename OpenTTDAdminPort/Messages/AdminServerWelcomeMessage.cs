using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    internal record AdminServerWelcomeMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_WELCOME;

        public string ServerName { get; init; } = default!;

        public string NetworkRevision { get; init; } = default!;

        public bool IsDedicated { get; init; }

        public string MapName { get; init; } = default!;

        public uint MapSeed { get; init; }

        public Landscape Landscape { get; init; }

        public OttdDate CurrentDate { get; init; } = default!;

        public ushort MapWidth { get; init; }

        public ushort MapHeight { get; init; }
    }
}
