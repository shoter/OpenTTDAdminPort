using System;

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
        public void ConnectingState()
        {
            OnTransition((prevState, newState) =>
            {
                if (newState == MainState.Connecting)
                {
                    Stash.ClearStash();

                    logger.LogTrace("Initializing connecting state");

                    ConnectingData data = (NextStateData as ConnectingData)!;
                    var msg = new AdminJoinMessage(data.ServerInfo.Password, data.ClientName, this.version);
                    data.TcpClient.Tell(new SendMessage(msg));

                    var checkIfConnectedMsg = new AdminPortCheckIfConnected(data.UniqueConnectingIdentifier);
                    Timers.StartSingleTimer(data.UniqueConnectingIdentifier, checkIfConnectedMsg, 3.Seconds());
                }
            });

            When(MainState.Connecting, state =>
            {
                ConnectingData data = (state.StateData as ConnectingData)!;

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

                                foreach (var s in msg.AdminUpdateSettings)
                                {
                                    data.AdminUpdateSettings.Add(s.Key, new AdminUpdateSetting(true, s.Key, s.Value));
                                }

                                return Stay();
                            }

                        case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
                            {
                                var msg = (AdminServerWelcomeMessage)message;

                                data.AdminServerInfo = new AdminServerInfo(
                                    msg.ServerName,
                                    msg.NetworkRevision,
                                    msg.IsDedicated,
                                    msg.MapName,
                                    msg.CurrentDate,
                                    msg.Landscape,
                                    msg.MapWidth,
                                    msg.MapHeight);

                                IActorRef watchdog = actorFactory.CreateWatchdog(Context, data.TcpClient, 5.Seconds());

                                logger.LogTrace($"Moving {data.Initiator} to Connected state");
                                data.Initiator.Tell(SuccessResponse.Instance);
                                this.Messager.Tell(new AdminServerConnected());
                                SendUpdateFreqs(data.TcpClient);
                                return GoTo(MainState.Connected).Using(new ConnectedData(data, watchdog));
                            }
                    }
                }
                else if (state.FsmEvent is AdminPortTcpClientConnectionLostException)
                {
                    return RestartConnecting(data);
                }
                else if(state.FsmEvent is AdminPortCheckIfConnected checkIfConnected)
                {
                    if (checkIfConnected.ConnectingId == data.UniqueConnectingIdentifier)
                    {
                        logger.LogTrace("Could not connect within 3 seconds. Restarting connection attempt");
                        return RestartConnecting(data);
                    }
                }
                else if (state.FsmEvent is FatalTcpClientException)
                {
                    return RestartConnecting(data);
                }

                return null;
            });
        }

        private State<MainState, IMainData> RestartConnecting(ConnectingData data)
        {
            data.TcpClient.GracefulStop(3.Seconds()).Wait();
            IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
            this.Messager.Tell(new AdminServerConnectionLost());
            return GoTo(MainState.Connecting).Using(new ConnectingData(tcpClient, data.Initiator, data.ServerInfo, data.ClientName));
        }

        private void SendUpdateFreqs(IActorRef tcpClient)
        {
            var dateFreq = new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_DATE, UpdateFrequency.ADMIN_FREQUENCY_MONTHLY);
            tcpClient.Tell(new SendMessage(dateFreq));
        }
    }
}
