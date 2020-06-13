namespace OpenTTDAdminPort.Events
{
    public class AdminPongEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.Pong;

        public ServerInfo Server { get; }

        public uint PongValue { get; }

        public AdminPongEvent(ServerInfo server, uint pongValue)
        {
            this.Server = server;
            this.PongValue = pongValue;
        }
    }
}
