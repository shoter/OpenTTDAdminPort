using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public record AdminClientJoinEvent(Player Player) : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ClientJoin;
    }
}
