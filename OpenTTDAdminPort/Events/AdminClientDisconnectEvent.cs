using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminClientDisconnectEvent : IAdminEvent
    {
        public Player Player { get; }

        public string Reason { get; }

        public AdminEventType EventType => AdminEventType.ClientDisconnect;

        public AdminClientDisconnectEvent(Player player, string reason)
        {
            this.Player = player;
            this.Reason = reason;
        }
    }
}
