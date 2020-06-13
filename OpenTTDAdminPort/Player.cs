namespace OpenTTDAdminPort
{
    public class Player
    {
        public uint ClientId { get; }
        public string Name { get; set; }

        public Player(uint clientId) => this.ClientId = clientId;

        public Player(uint clientId, string name)
            : this(clientId) => this.Name = name;

    }
}
