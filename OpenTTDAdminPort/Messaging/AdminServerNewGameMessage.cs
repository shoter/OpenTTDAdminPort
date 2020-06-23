namespace OpenTTDAdminPort.Messaging
{
    public class AdminServerNewGameMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_NEWGAME;
    }
}
