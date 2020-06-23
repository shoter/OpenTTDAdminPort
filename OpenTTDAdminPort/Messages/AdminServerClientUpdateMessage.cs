namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerClientUpdateMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE;

        public uint ClientId { get; }

        public string ClientName { get; }

        public byte PlayingAs { get; }

        public AdminServerClientUpdateMessage(uint clientId, string clientName, byte playingAs)
        {
            this.ClientId = clientId;
            this.ClientName = clientName;
            this.PlayingAs = playingAs;
        }
    }
}
