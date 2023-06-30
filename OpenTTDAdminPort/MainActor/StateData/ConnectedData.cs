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
        byte AdminPortNetworkVersion,
        IReadOnlyDictionary<uint, Player> Players,
        IReadOnlyDictionary<byte, Company> Companies,
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
                      data.AdminPortNetworkVersion!.Value,
                      new Dictionary<uint, Player>(),
                      new Dictionary<byte, Company>(),
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

        public ConnectedData UpsertCompany(Company company)
        {
            var companies = new Dictionary<byte, Company>(Companies);
            companies[company.Id] = company;
            return this with { Companies = companies };
        }

        public ConnectedData RemoveCompany(byte companyId)
        {
            var companies = new Dictionary<byte, Company>(Companies);
            companies.Remove(companyId);
            return this with { Companies = companies };
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
