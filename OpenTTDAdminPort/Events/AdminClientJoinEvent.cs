using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminClientJoinEvent : IAdminEvent
    {
        public Player Player { get; }

        public AdminEventType EventType => AdminEventType.ClientUpdate;

        public AdminClientJoinEvent(Player player)
        {
            this.Player = player;
        }
    }
}
