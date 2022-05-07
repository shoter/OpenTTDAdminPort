namespace OpenTTDAdminPort.Events
{
    public class AdminServerConnectionLost : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ServerConnectionLost;
    }
}
