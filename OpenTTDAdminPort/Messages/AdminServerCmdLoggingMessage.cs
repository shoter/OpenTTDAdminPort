namespace OpenTTDAdminPort.Messages
{
    public class AdminServerCmdLoggingMessage : IAdminMessage
    {
        public uint ClientId { get; set; }

        public byte CompanyId { get; set; }

        public ushort Cmd { get; set; }

        public uint P1 { get; set; }

        public uint P2 { get; set; }

        public uint Tile { get; set; }

        public string? Text { get; set; }

        public uint Frame { get; set; }

        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_LOGGING;
    }
}
