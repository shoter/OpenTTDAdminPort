namespace OpenTTDAdminPort.Messages
{
    internal class AdminServerPongMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;

        public uint Argument { get; }

        public AdminServerPongMessage(uint arg) => Argument = arg;

        public override string ToString() => $"AdminServerPongMessage({Argument})";
    }
}
