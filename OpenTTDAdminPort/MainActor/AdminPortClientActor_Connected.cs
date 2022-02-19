using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
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

                    Context.Parent.Tell(receive);
                }
                else if (state.FsmEvent is AdminPortDisconnect)
                {
                    killChildren(data);

                    return GoTo(MainState.Idle).Using(new IdleData());
                }
                else if (state.FsmEvent is WatchdogConnectionLost)
                {
                    killChildren(data);
                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, data.ServerInfo.ServerIp, data.ServerInfo.ServerPort);
                    return GoTo(MainState.Connecting).Using(new ConnectingData(tcpClient, data.ServerInfo, data.ClientName));
                }

                return null;
            });
        }

        protected override SupervisorStrategy ConnectedSupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case ArithmeticException ae:
                            return Directive.Resume;
                        case NullReferenceException nre:
                            return Directive.Restart;
                        case ArgumentException are:
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
    });

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
