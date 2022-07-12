using OpenTTDAdminPort.Game;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. RequiredMember is used here to mitigate that.
namespace OpenTTDAdminPort.Messages
{
    internal record AdminServerClientInfoMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;

        public uint ClientId { get; set; }

        public string Hostname { get; set; }

        public string ClientName { get; set; }

        public byte Language { get; set; }

        public OttdDate JoinDate { get; set; }

        public byte PlayingAs { get; set; }
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

