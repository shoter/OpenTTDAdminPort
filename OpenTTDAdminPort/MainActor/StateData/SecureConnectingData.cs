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
    public record SecureConnectingData(
        IActorRef TcpClient,
        IActorRef Initiator,
        ServerInfo ServerInfo,
        string ClientName,
        Guid UniqueConnectingIdentifier,
        byte[]? ServerPublicKey,
        byte[]? Nonce,
        byte[]? ClientSecretKey,
        byte[]? ClientPublicKey,
        byte[]? ServerToClientKey,
        byte[]? ClientToServerKey,
        byte[]? ChallengeMessage) : IMainData
    {
        public SecureConnectingData(
            IActorRef tcpClient,
            IActorRef initiator,
            ServerInfo serverInfo,
            string clientName)
        : this(
            tcpClient,
            initiator,
            serverInfo,
            clientName,
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null,
            null,
            null
            )
        {
        }
    }
}