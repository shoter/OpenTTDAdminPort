using System;

namespace OpenTTDAdminPort.Game
{
    public class Player
    {
        public uint ClientId { get; }
        public string Name { get; set; }

        public DateTimeOffset ConnectedTime { get; } = new DateTimeOffset();

        public Player(uint clientId, string name, DateTimeOffset connectedTime)
        {
            this.ClientId = clientId;
            this.Name = name;
            this.ConnectedTime = connectedTime;
        }

    }
}
