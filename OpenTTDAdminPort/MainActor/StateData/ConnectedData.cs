using System.Collections.Generic;
using System.Diagnostics;

using Akka.Actor;
using Newtonsoft.Json;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;

namespace OpenTTDAdminPort.MainActor.StateData
{
    public record ConnectedData(
        [property: JsonIgnore] IActorRef TcpClient,
        ServerInfo ServerInfo,
        string ClientName,
        [property: JsonIgnore] IActorRef Watchdog,
        IReadOnlyDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings,
        AdminServerInfo AdminServerInfo,
        IReadOnlyDictionary<uint, Player> Players,
        IReadOnlyDictionary<uint, Player> PastPlayers) : IMainData
    {
        public ConnectedData(ConnectingData data, IActorRef watchdog)
                : this(
                      data.TcpClient,
                      data.ServerInfo,
                      data.ClientName,
                      watchdog,
                      data.AdminUpdateSettings,
                      data.AdminServerInfo!,
                      new Dictionary<uint, Player>(),
                      new Dictionary<uint, Player>())
        {
            Debug.Assert(data.AdminServerInfo != null);
        }

        public ConnectedData UpsertPlayer(Player player)
        {
            var players = new Dictionary<uint, Player>(Players);
            players[player.ClientId] = player;
            return this with { Players = players };
        }

        public ConnectedData DeletePlayer(uint clientId)
        {
            var players = new Dictionary<uint, Player>(Players);
            var pastPlayers = PastPlayers;

            if(players.TryGetValue(clientId, out Player? player))
            {
                pastPlayers = new Dictionary<uint, Player>(PastPlayers)
                {
                    { clientId, player },
                };
            }

            players.Remove(clientId);
            return this with { Players = players, PastPlayers = pastPlayers };
        }

        public Player GetPlayerFromAll(uint clientId)
        {
            if(Players.TryGetValue(clientId, out Player? player))
            {
                return player;
            }

            return PastPlayers[clientId];
        }
    }
}
