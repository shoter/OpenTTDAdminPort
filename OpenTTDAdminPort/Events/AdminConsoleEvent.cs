namespace OpenTTDAdminPort.Events
{
    public class AdminConsoleEvent : IAdminEvent
    {
        public string Origin { get; }

        public string Message { get; }

        public AdminEventType EventType => AdminEventType.ConsoleMessage;

        public AdminConsoleEvent(string origin, string message)
        {
            this.Origin = origin;
            this.Message = message;
        }
    }
}
