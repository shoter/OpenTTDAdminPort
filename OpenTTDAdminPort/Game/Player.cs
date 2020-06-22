namespace OpenTTDAdminPort.Game
{
    public class Player
    {
        public uint ClientId { get; }
        public string Name { get; set; }

        public Player(uint clientId, string name)
        {
            this.ClientId = clientId;
            this.Name = name;
        }

    }
}
