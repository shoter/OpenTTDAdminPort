using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminClientInfoEvent : IAdminEvent
    {
        public string Hostname { get; }

        public OttdDate JoinDate { get; }


        public byte PlayingAs { get; }

        public Player Player { get; }

        public AdminEventType EventType => AdminEventType.ClientUpdate;

        public AdminClientInfoEvent(Player player, OttdDate joinDate, byte playingAs, string hostName)
        {
            this.Player = player;
            this.Hostname = hostName;
            this.PlayingAs = playingAs;
            this.JoinDate = joinDate;
        }
    }
}
