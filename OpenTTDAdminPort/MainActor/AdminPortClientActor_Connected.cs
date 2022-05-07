using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Exceptions;
using OpenTTDAdminPort.Networking.Watchdog;

using System;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash
    {
        public void ConnectedState()
        {
            OnTransition((prevState, newState) =>
            {
                if (newState == MainState.Connected)
                {


                    logger.LogTrace("Connected state initialized. Unstashing all messages.");
                    Stash.UnstashAll();
                }
            });

            When(MainState.Connected, state =>
            {
                ConnectedData data = (state.StateData as ConnectedData)!;

                if (state.FsmEvent is SendMessage send)
                {
                    logger.LogTrace($"Sending {send.Message} to TcpClient");
                    data.TcpClient.Tell(send);
                }
                else if (state.FsmEvent is ReceiveMessage receive)
                {
                    logger.LogTrace($"Received {receive.Message} - sending to Parent");
                    ProcessAdminMessage(data, receive.Message);

                    IAdminEvent? ev = this.adminEventFactory.Create(receive.Message, data);

                    if (ev != null)
                    {
                        this.Messager.Tell(ev);
                    }

                    return Stay();
                }
                else if (state.FsmEvent is AdminPortDisconnect)
                {
                    killChildren(data);

                    return GoTo(MainState.Idle).Using(new IdleData()).Replying(EmptyResponse.Instance);
                }
                else if (state.FsmEvent is WatchdogConnectionLost || state.FsmEvent is AdminPortTcpClientConnectionLostException)
                {
                    killChildren(data);
                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
                    this.Messager.Tell(new AdminServerConnectionLost());
                    return GoTo(MainState.Connecting).Using(new ConnectingData(tcpClient, Self, data.ServerInfo, data.ClientName));
                }
                else if(state.FsmEvent is AdminPortQueryState queryState)
                {
                    return Stay().Replying(new AdminPortReponseState(queryState, new MainActorState(data)));
                }

                return null;
            });
        }

        private void ProcessAdminMessage(ConnectedData data, IAdminMessage message)
        {
            switch (message)
            {
                case AdminServerClientInfoMessage ci:
                    {
                        data.Players.Add(ci.ClientId, new Player(ci.ClientId, ci.ClientName, DateTimeOffset.Now, ci.Hostname, ci.PlayingAs));
                        break;
                    }
                case AdminServerClientUpdateMessage cu:
                    {
                        var player = data.Players[cu.ClientId];
                        player.Name = cu.ClientName;
                        player.PlayingAs = cu.PlayingAs;
                        break;
                    }
                case AdminServerClientQuitMessage cq:
                    {
                        data.Players.Remove(cq.ClientId);
                        break;
                    }
                default:
                    {
                        // Other messages are not relevant to update server state
                        break;
                    }
            }

        }

        private static void killChildren(ConnectedData data)
        {
            Task[] killTasks = new Task[]
            {
                data.TcpClient.GracefulStop(3.Seconds()),
                data.Watchdog.GracefulStop(3.Seconds())
            };
            Task.WaitAll(killTasks);
        }
    }
}
