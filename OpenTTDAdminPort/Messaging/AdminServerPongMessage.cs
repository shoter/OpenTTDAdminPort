namespace OpenTTDAdminPort.Messaging
{
    internal class AdminServerPongMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;

        public uint Argument { get; }

        public AdminServerPongMessage(uint arg) => Argument = arg;
    }
}
