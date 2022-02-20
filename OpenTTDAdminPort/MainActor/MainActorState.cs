using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;

using System.Collections.Generic;

namespace OpenTTDAdminPort.MainActor
{
    public class MainActorState
    {
        public Dictionary<uint, Player> Players { get; } = new();

        public MainActorState(ConnectedData data)
        {
            foreach(var player in data.Players.Values)
            {
                Players.Add(player.ClientId, player.Copy());
            }
        }
    }
}
