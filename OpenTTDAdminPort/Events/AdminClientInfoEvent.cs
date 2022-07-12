using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminClientInfoEvent : IAdminEvent
    {
        public OttdDate JoinDate { get; }

        public Player Player { get; }

        public AdminEventType EventType => AdminEventType.ClientUpdate;

        public AdminClientInfoEvent(Player player, OttdDate joinDate)
        {
            this.Player = player;
            this.JoinDate = joinDate;
        }
    }
}
