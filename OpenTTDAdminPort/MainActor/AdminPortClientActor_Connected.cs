using System;
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
                    ConnectedData newData = ProcessAdminMessage(data, receive.Message);

                    IAdminEvent? ev = this.adminEventFactory.Create(receive.Message, data, newData);

                    if (ev != null)
                    {
                        this.Messager.Tell(ev);

                        foreach (var waiter in waiterActors)
                        {
                            waiter.Tell(ev);
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
                else if(state.FsmEvent is QueryServerStatus)
                {
                    Sender.Tell(new ServerStatus(data.AdminServerInfo, data.Players));
                }

                return null;
            });
        }

        private ConnectedData ProcessAdminMessage(ConnectedData data, IAdminMessage message)
        {
            switch (message)
            {
                case AdminServerClientInfoMessage ci:
                    {
                        var player = new Player(ci.ClientId, ci.ClientName, DateTimeOffset.Now, ci.Hostname, ci.PlayingAs);
                        return data.UpsertPlayer(player);
                    }

                case AdminServerClientUpdateMessage cu:
                    {
                        var player = data.Players[cu.ClientId];
                        return data.UpsertPlayer(player with
                        {
                            Name = cu.ClientName,
                            PlayingAs = cu.PlayingAs,
                        });
                    }

                case AdminServerClientQuitMessage cq:
                    {
                        // This if here is just in case if there is a chance that we try to remove player that was somehow not registered by the server.
                        // You can call it defensive programming or smth.
                        if (data.Players.ContainsKey(cq.ClientId))
                        {
                            return data.DeletePlayer(cq.ClientId);
                        }

                        return data;
                    }

                case AdminServerClientErrorMessage em:
                    {
                        // There is an error in some version of openttd (mayeb in all?) where after client disconnects and server successfully
                        // sends quitMessage server is going to send error message.
                        // This is errorneous behaviour and if client is already disconnected we are going to ignor `error` messages connected with him.
                        if (data.Players.ContainsKey(em.ClientId))
                        {
                            return data.DeletePlayer(em.ClientId);
                        }

                        return data;
                    }

                case AdminServerCompanyInfoMessage cim:
                    {
                        Company company = new Company(
                                cim.CompanyId,
                                cim.CompanyName,
                                cim.ManagerName,
                                cim.Color,
                                cim.HasPassword,
                                cim.CreationDate,
                                cim.IsAi,
                                cim.MonthsOfBankruptcy
                                );

                        return data.UpsertCompany(company);
                    }

                case AdminServerCompanyUpdateMessage cum:
                    {
                        Company company = data.Companies[cum.CompanyId] with 
                        {
                            Name = cum.CompanyName,
                            ManagerName = cum.ManagerName,
                            Color = cum.Color,
                            HasPassword = cum.HasPassword,
                            MonthsOfBankruptcy = cum.MonthsOfBankruptcy
                        };

                        return data.UpsertCompany(company);
                    }

                case AdminServerCompanyRemoveMessage crm:
                    {
                        return data.RemoveCompany(crm.CompanyId);
                    }

                case AdminServerDateMessage dateMsg:
                    {
                        return data with
                        {
                            AdminServerInfo = data.AdminServerInfo with
                            {
                                Date = dateMsg.Date,
                            },
                        };
                    }

                default:
                    {
                        // Other messages are not relevant to update server state
                        return data;
                    }
            }
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
