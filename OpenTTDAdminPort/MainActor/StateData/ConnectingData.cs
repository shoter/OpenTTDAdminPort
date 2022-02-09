using Akka.Actor;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;

using System.Collections.Concurrent;

namespace OpenTTDAdminPort.MainActor.StateData
{
    public class ConnectingData : IMainData
    {
        public IActorRef TcpClient { get; }

        public ServerInfo ServerInfo { get; }

        public string ClientName { get; }

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new();

        public AdminServerInfo? AdminServerInfo { get; set; }

        public ConnectingData(IActorRef tcpClient,ServerInfo serverInfo, string clientName)
        {
            this.TcpClient = tcpClient;
            this.ServerInfo = serverInfo;
            this.ClientName = clientName;
        }
    }
}
