using System;

namespace OpenTTDAdminPort.Game
{
    public class Player
    {
        public uint ClientId { get; }

        public string Name { get; set; }

        public string Hostname { get; }

        public byte PlayingAs { get; set; }

        public DateTimeOffset ConnectedTime { get; } = default(DateTimeOffset);

        public Player(uint clientId, string name, DateTimeOffset connectedTime, string hostName, byte playingAs)
        {
            this.ClientId = clientId;
            this.Name = name;
            this.ConnectedTime = connectedTime;
            this.Hostname = hostName;
            this.PlayingAs = playingAs;
        }

        public Player Copy()
        {
            return new Player(this.ClientId, this.Name, this.ConnectedTime, this.Hostname, this.PlayingAs);
        }
    }
}
