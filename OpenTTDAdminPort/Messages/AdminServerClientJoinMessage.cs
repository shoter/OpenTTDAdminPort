namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerClientJoinMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN;

        public uint ClientId { get; }

        public AdminServerClientJoinMessage(uint clientId) => ClientId = clientId;
    }
}
