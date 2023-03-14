using System;

namespace OpenTTDAdminPort.Game
{
    public record Player(
    uint ClientId,
    string Name,
    string Hostname,
    byte PlayingAs,
    DateTimeOffset ConnectedTime)
    {
        public Player(uint clientId, string name, DateTimeOffset connectedTime, string hostName, byte playingAs)
            : this(clientId, name, hostName, playingAs, connectedTime)
        {
        }

        public Player Copy()
        {
            return new Player(this.ClientId, this.Name, this.ConnectedTime, this.Hostname, this.PlayingAs);
        }
    }
}
