namespace OpenTTDAdminPort.Events
{
    public class AdminServerRestarted : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ServerRestarted;
    }
}
