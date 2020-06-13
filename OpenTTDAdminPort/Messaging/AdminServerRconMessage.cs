namespace OpenTTDAdminPort.Messaging
{
    public class AdminServerRconMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_RCON;

        public ushort Color { get; }

        public string Result { get; }

        public AdminServerRconMessage(ushort color, string result)
        {
            this.Color = color;
            this.Result = result;
        }
    }
}
