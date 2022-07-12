using RandomAnalyzers.RequiredMember;

namespace OpenTTDAdminPort.Messages
{
    public class AdminServerCmdLoggingMessage : IAdminMessage
    {
        [RequiredMember]
        public uint ClientId { get; set; }

        [RequiredMember]
        public byte CompanyId { get; set; }

        [RequiredMember]
        public ushort Cmd { get; set; }

        [RequiredMember]
        public uint P1 { get; set; }

        [RequiredMember]
        public uint P2 { get; set; }

        [RequiredMember]
        public uint Tile { get; set; }

        [RequiredMember]
        public string? Text { get; set; }

        [RequiredMember]
        public uint Frame { get; set; }

        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_LOGGING;
    }
}
