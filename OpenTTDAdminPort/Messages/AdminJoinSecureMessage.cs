namespace OpenTTDAdminPort.Messages
{
    public record AdminJoinSecureMessage(string AdminName, string AdminVersion)
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_JOIN_SECURE;
    }
}