namespace OpenTTDAdminPort.Events
{
    public class AdminRconEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.AdminRcon;

        public string Message { get; }

        public AdminRconEvent(string msg)
        {
            this.Message = msg;
        }
    }
}
