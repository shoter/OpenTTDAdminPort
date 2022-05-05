using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

using System;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash
    {
        public void ConnectingState()
        {
            OnTransition((prevState, newState) =>
            {
                if(newState == MainState.Connecting)
                {
                    Stash.ClearStash();

                    logger.LogTrace("Initializing connecting state");

                    ConnectingData data = (NextStateData as ConnectingData)!;
                    var msg = new AdminJoinMessage(data.ServerInfo.Password, data.ClientName, this.version);
                    data.TcpClient.Tell(new SendMessage(msg));
                }
            });

            When(MainState.Connecting, state =>
            {
                ConnectingData data = (state.StateData as ConnectingData)!;

                if(state.FsmEvent is AdminPortDisconnect)
                {
                    logger.LogTrace("Disconnecting admin port client");
                    data.TcpClient.GracefulStop(3.Seconds()).Wait();
                    return GoTo(MainState.Idle).Using(new IdleData()).Replying(AdminPortDisconnected.Instance);
                }
                else if(state.FsmEvent is ReceiveMessage rec)
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

                                data.AdminServerInfo = new AdminServerInfo()
                                {
                                    IsDedicated = msg.IsDedicated,
                                    MapName = msg.MapName,
                                    RevisionName = msg.NetworkRevision,
                                    ServerName = msg.ServerName
                                };

                                IActorRef watchdog = actorFactory.CreateWatchdog(Context, data.TcpClient, 5.Seconds());

                                logger.LogTrace("Moving to Connected state");
                                data.Initiator.Tell(SuccessResponse.Instance);
                                return GoTo(MainState.Connected).Using(new ConnectedData(data, watchdog));
                            }
                    }

                } else if(state.FsmEvent is SendMessage m)
                {
                    // Let's handle it later when we connect to the server.
                    Stash.Stash();
                }

                return null;
            });

        }

    }
}
