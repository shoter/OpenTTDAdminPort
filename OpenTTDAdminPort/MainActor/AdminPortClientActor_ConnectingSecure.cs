using System;
using System.Collections.Generic;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash, IWithTimers
    {
        public void ConnectingSecureState()
        {
            OnTransition((prevState, newState) =>
            {
                if (newState == MainState.ConnectingSecure)
                {
                    Stash.ClearStash();

                    logger.LogTrace("Initializing secure connecting state");

                    SecureConnectingData data = (NextStateData as SecureConnectingData)!;
                    var msg = new AdminJoinSecureMessage(data.ClientName, this.version);
                    data.TcpClient.Tell(new SendMessage(msg));

                    var checkIfConnectedMsg = new AdminPortCheckIfConnected(data.UniqueConnectingIdentifier);
                    Timers.StartSingleTimer(data.UniqueConnectingIdentifier, checkIfConnectedMsg, 3.Seconds());
                }
            });

            When(
                MainState.ConnectingSecure,
                state =>
                {
                    SecureConnectingData data = (state.StateData as SecureConnectingData)!;

                    if (state.FsmEvent is AdminPortDisconnect)
                    {
                        logger.LogTrace("Disconnecting admin port client");
                        data.TcpClient.GracefulStop(3.Seconds())
                            .Wait();
                        return GoTo(MainState.Idle)
                            .Using(new IdleData())
                            .Replying(AdminPortDisconnected.Instance);
                    }
                    else if (state.FsmEvent is ReceiveMessage rec)
                    {
                        var message = rec.Message;
                        logger.LogTrace($"Received message {message.MessageType}");
                        switch (message.MessageType)
                        {
                            case AdminMessageType.ADMIN_PACKET_SERVER_AUTH_REQUEST:
                            {
                                var msg = (AdminServerAuthRequest)message;
                                var keyPair = AdminPortCrypto.GenerateKeyPair();

                                var exchange = AdminPortCrypto.PerformKeyExchange(
                                    msg.ServerPublicKey,
                                    keyPair.SecretKey,
                                    keyPair.PublicKey,
                                    data.ServerInfo.Password
                                );

                                if (exchange == null)
                                {
                                    logger.LogTrace($"Something is very wrong. Exchange failed");
                                    return RestartSecureConnecting(data);
                                }

                                byte[] challenge = new byte[8];
                                Random.Shared.NextBytes(challenge);

                                var encrypt = AdminPortCrypto.EncryptAuthChallenge(
                                    challenge,
                                    exchange.ClientToServerKey,
                                    msg.Nonce,
                                    keyPair.PublicKey);

                                var responseMessage = new AdminAuthResponseMessage(
                                    keyPair.PublicKey,
                                    encrypt.Mac,
                                    encrypt.Ciphertext);

                                data.TcpClient.Tell(new SendMessage(responseMessage));

                                return Stay()
                                    .Using(
                                        data with
                                        {
                                            ServerPublicKey = msg.ServerPublicKey,
                                            Nonce = msg.Nonce,
                                            ClientPublicKey = keyPair.PublicKey,
                                            ClientSecretKey = keyPair.SecretKey,
                                            ClientToServerKey = exchange.ClientToServerKey,
                                            ServerToClientKey = exchange.ServerToClientKey,
                                            ChallengeMessage = challenge,
                                        });
                            }

                            case AdminMessageType.ADMIN_PACKET_SERVER_ENABLE_ENCRYPTION:
                            {
                                var msg = (AdminServerEnableEncryptionMessage)message;

                                var encryptionHandler = new AdminPortCrypto.PacketEncryptionHandler(
                                    msg.EncryptionNonce,
                                    data.ClientToServerKey!,
                                    data.ServerToClientKey!);

                                data.TcpClient.Tell(new StartEncryptedConnectionMessage(encryptionHandler));


                                var connectingData = new ConnectingData(data.TcpClient, Sender, data.ServerInfo, data.ClientName);

                                logger.LogTrace("Moving to Connecting state");
                                return GoTo(MainState.Connecting).Using(connectingData);
                            }
                        }
                    }
                    else if (state.FsmEvent is AdminPortTcpClientConnectionLostException)
                    {
                        return RestartSecureConnecting(data);
                    }
                    else if (state.FsmEvent is AdminPortCheckIfConnected checkIfConnected)
                    {
                        if (checkIfConnected.ConnectingId == data.UniqueConnectingIdentifier)
                        {
                            logger.LogTrace("Could not connect within 3 seconds. Restarting connection attempt");
                            return RestartSecureConnecting(data);
                        }
                    }
                    else if (state.FsmEvent is FatalTcpClientException)
                    {
                        return RestartSecureConnecting(data);
                    }

                    return null;
                });
        }

        private State<MainState, IMainData> RestartSecureConnecting(SecureConnectingData data)
        {
            try
            {
                data.TcpClient.GracefulStop(3.Seconds())
                    .Wait();
            }
            catch
            {
                // ignoring :(
            }

            IActorRef tcpClient = actorFactory.CreateTcpClient(
                Context,
                data.ServerInfo.ServerIp,
                data.ServerInfo.ServerPort);
            this.Messager.Tell(new AdminServerConnectionLost());
            return GoTo(MainState.ConnectingSecure)
                .Using(
                    new SecureConnectingData(
                        tcpClient,
                        data.Initiator,
                        data.ServerInfo,
                        data.ClientName));
        }
    }
}