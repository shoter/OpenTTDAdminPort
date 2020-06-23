namespace OpenTTDAdminPort.Messages
{
    public class AdminServerShutdownMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_SHUTDOWN;
    }
}
