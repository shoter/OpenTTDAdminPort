using OpenTTDAdminPort.Game;
using RandomAnalyzers.RequiredMember;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. RequiredMember is used here to mitigate that.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

