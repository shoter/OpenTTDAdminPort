using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Akka.Actor;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;

namespace OpenTTDAdminPort.MainActor.StateData
{
    /// <summary>
    /// Initiator of connect process to which we will send a message informing about successfull connect
    /// </summary>
    public record ConnectingData(
        IActorRef TcpClient,
        IActorRef Initiator,
        ServerInfo ServerInfo,
        string ClientName,
        IReadOnlyDictionary<AdminUpdateType, AdminUpdateSetting>? AdminUpdateSettings,
        AdminServerInfo? AdminServerInfo,
        Guid UniqueConnectingIdentifier,
        byte? AdminPortNetworkVersion) : IMainData
    {
        public ConnectingData(
            IActorRef tcpClient,
            IActorRef initiator,
            ServerInfo serverInfo,
            string clientName)
        : this(
            tcpClient,
            initiator,
            serverInfo,
            clientName,
            null,
            null,
            Guid.NewGuid(),
            null
            )
        {
        }
    }
}