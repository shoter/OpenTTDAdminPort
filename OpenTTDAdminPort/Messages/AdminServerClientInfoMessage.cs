using OpenTTDAdminPort.Game;
using RandomAnalyzers.RequiredMember;

namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerClientInfoMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;

        [RequiredMember]
        public uint ClientId { get; set; }

        [RequiredMember]

        public string Hostname { get; set; }

        [RequiredMember]

        public string ClientName { get; set; }

        [RequiredMember]

        public byte Language { get; set; }

        [RequiredMember]

        public OttdDate JoinDate { get; set; }

        [RequiredMember]

        public byte PlayingAs { get; set; }
    }
}
