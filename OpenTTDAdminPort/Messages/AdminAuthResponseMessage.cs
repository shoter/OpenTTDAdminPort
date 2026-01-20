namespace OpenTTDAdminPort.Messages
{
    public record AdminAuthResponseMessage(byte[] ClientPublicKey, byte[] Mac, byte[] CipherText) : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_AUTH_RESPONSE;
    }
}