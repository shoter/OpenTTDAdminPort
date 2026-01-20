namespace OpenTTDAdminPort.Messages
{
    // We are not going to include PAKE uint - we are ignoring it. We always use PAKE
    public record AdminServerAuthRequest(byte[] ServerPublicKey, byte[] Nonce) : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_AUTH_REQUEST;
    }
}