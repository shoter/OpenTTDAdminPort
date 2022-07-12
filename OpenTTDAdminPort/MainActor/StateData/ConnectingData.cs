using System.Collections.Concurrent;
using System.Collections.Generic;

using Akka.Actor;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;

namespace OpenTTDAdminPort.MainActor.StateData
{
    public class ConnectingData : IMainData
    {
        public IActorRef TcpClient { get; }

        /// <summary>
        /// Initiator of connect process to which we will send a message informing about successfull connect
        /// </summary>
        public IActorRef Initiator { get; }

        public ServerInfo ServerInfo { get; }

        public string ClientName { get; }

        public Dictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new();

        public AdminServerInfo? AdminServerInfo { get; set; }

        public ConnectingData(IActorRef tcpClient, IActorRef initiator, ServerInfo serverInfo, string clientName)
        {
            this.TcpClient = tcpClient;
            this.Initiator = initiator;
            this.ServerInfo = serverInfo;
            this.ClientName = clientName;
        }
    }
}
