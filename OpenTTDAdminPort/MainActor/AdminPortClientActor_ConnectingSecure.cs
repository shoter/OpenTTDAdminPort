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

            When(MainState.ConnectingSecure, state =>
            {
                SecureConnectingData data = (state.StateData as SecureConnectingData)!;

                if (state.FsmEvent is AdminPortDisconnect)
                {
                    logger.LogTrace("Disconnecting admin port client");
                    data.TcpClient.GracefulStop(3.Seconds()).Wait();
                    return GoTo(MainState.Idle).Using(new IdleData()).Replying(AdminPortDisconnected.Instance);
                }
                else if (state.FsmEvent is ReceiveMessage rec)
                {
                    var message = rec.Message;
                    logger.LogTrace($"Received message {message.MessageType}");
                    switch (message.MessageType)
                    {
                        case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
                            {
                                var msg = (AdminServerProtocolMessage)message;

                                Dictionary<AdminUpdateType, AdminUpdateSetting> adminUpdateSettings = new();

                                foreach (var s in msg.AdminUpdateSettings)
                                {
                                    adminUpdateSettings.Add(s.Key, new AdminUpdateSetting(true, s.Key, s.Value));
                                }

                                return Stay()
                                    .Using(
                                        data with
                                        {
                                            AdminUpdateSettings = adminUpdateSettings,
                                            AdminPortNetworkVersion = msg.NetworkVersion,
                                        });
                            }

                        case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
                            {
                                var msg = (AdminServerWelcomeMessage)message;

                                var newData = data with
                                {
                                    AdminServerInfo = new AdminServerInfo(
                                        msg.ServerName,
                                        msg.NetworkRevision,
                                        msg.IsDedicated,
                                        msg.MapName,
                                        msg.CurrentDate,
                                        msg.Landscape,
                                        msg.MapWidth,
                                        msg.MapHeight),
                                };

                                IActorRef watchdog = actorFactory.CreateWatchdog(Context, data.TcpClient, 5.Seconds());

                                logger.LogTrace($"Moving {data.Initiator} to Connected state");
                                data.Initiator.Tell(SuccessResponse.Instance);
                                this.Messager.Tell(new AdminServerConnected());
                                SendUpdateFreqs(data.TcpClient);
                                return GoTo(MainState.Connected).Using(new ConnectedData(newData, watchdog));
                            }
                    }
                }
                else if (state.FsmEvent is AdminPortTcpClientConnectionLostException)
                {
                    return RestartSecureConnecting(data);
                }
                else if(state.FsmEvent is AdminPortCheckIfConnected checkIfConnected)
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
                data.TcpClient.GracefulStop(3.Seconds()).Wait();
            }
            catch
            {
                // ignoring :(
            }

            IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
            this.Messager.Tell(new AdminServerConnectionLost());
            return GoTo(MainState.ConnectingSecure).Using(new SecureConnectingData(tcpClient, data.Initiator, data.ServerInfo, data.ClientName));
        }
    }
}