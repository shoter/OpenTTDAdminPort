namespace OpenTTDAdminPort.Events
{
    public class AdminPongEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.Pong;

        public uint PongValue { get; }

        public AdminPongEvent(uint pongValue)
        {
            this.PongValue = pongValue;
        }

        public override string ToString() => $"AdminPongEvent({PongValue})";
    }
}
