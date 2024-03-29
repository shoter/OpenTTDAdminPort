﻿using System;
using System.Threading.Tasks;

using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Networking.Watchdog;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash, IWithTimers
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
                    ConnectedData newData = incomingMessageProcessor.ProcessAdminMessage(
                        data,
                        receive.Message);

                    IAdminEvent? ev = this.adminEventFactory.Create(receive.Message, data, newData);

                    if (ev != null)
                    {
                        this.Messager.Tell(ev);

                        foreach (var waiter in waiterActors)
                        {
                            waiter.Value.Tell(ev);
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Could create admin event message for {receive.Message.MessageType}");
                    }

                    return Stay().Using(newData);
                }
                else if (state.FsmEvent is AdminPortDisconnect)
                {
                    KillChildren(data);

                    return GoTo(MainState.Idle).Using(new IdleData()).Replying(EmptyResponse.Instance);
                }
                else if (state.FsmEvent is WatchdogConnectionLost || state.FsmEvent is AdminPortTcpClientConnectionLostException)
                {
                    KillChildren(data);
                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
                    this.Messager.Tell(new AdminServerConnectionLost());
                    logger.LogError($"Connection to ${data.ServerInfo.ServerIp}:{data.ServerInfo.ServerPort}");
                    return GoTo(MainState.Connecting)
                           .Using(new ConnectingData(tcpClient, Self, data.ServerInfo, data.ClientName));
                }
                else if (state.FsmEvent is AdminPortQueryState queryState)
                {
                    return Stay().Replying(new AdminPortReponseState(queryState, new MainActorState(data)));
                }
                else if (state.FsmEvent is FatalTcpClientException)
                {
                    KillChildren(data);

                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
                    this.Messager.Tell(new AdminServerConnectionLost());
                    return GoTo(MainState.Connecting).Using(new ConnectingData(tcpClient, Self, data.ServerInfo, data.ClientName)).Replying(EmptyResponse.Instance);
                }
                else if (state.FsmEvent is QueryServerStatus)
                {
                    Sender.Tell(new ServerStatus(data.AdminServerInfo, data.Players));
                }

                return null;
            });
        }

        private static void KillChildren(ConnectedData data)
        {
            Task[] killTasks = new Task[]
            {
                data.TcpClient.GracefulStop(3.Seconds()),
                data.Watchdog.GracefulStop(3.Seconds()),
            };
            Task.WaitAll(killTasks);
        }
    }
}
