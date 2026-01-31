namespace OpenTTDAdminPort.Messages
{
    public record AdminServerEnableEncryptionMessage(byte[] EncryptionNonce) : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_ENABLE_ENCRYPTION;
    }
}