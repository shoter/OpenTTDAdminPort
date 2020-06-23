namespace OpenTTDAdminPort.Messages
{
    public class AdminPingMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_PING;

        public uint Argument { get; set; }

        public AdminPingMessage(uint argument = 0)
        {
            Argument = argument;
        }
    }
}
