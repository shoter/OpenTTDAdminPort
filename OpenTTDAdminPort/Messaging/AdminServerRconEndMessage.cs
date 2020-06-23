namespace OpenTTDAdminPort.Messaging
{
    public class AdminServerRconEndMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_RCON_END;

        public string Command { get; }

        public AdminServerRconEndMessage(string command)
        {
            this.Command = command;
        }
    }
}
