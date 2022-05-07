using System;

namespace OpenTTDAdminPort.Events
{
    public class AdminServerConnected : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ServerConnected;
    }
}
