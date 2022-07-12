namespace OpenTTDAdminPort.Messages
{
    public class AdminQuitMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_QUIT;
    }
}
