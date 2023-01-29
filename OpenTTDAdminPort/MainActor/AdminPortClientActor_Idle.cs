using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash, IWithTimers
    {
        public void IdleState()
        {
            When(MainState.Idle, state =>
            {
                if (state.FsmEvent is AdminPortConnect connect)
                {
                    try
                    {
                        logger.LogTrace($"I {Self} Received connect message from {Sender} to {connect.ServerInfo}");

                        IActorRef tcpClient = actorFactory.CreateTcpClient(Context, connect.ServerInfo.ServerIp, connect.ServerInfo.ServerPort);
                        var stateData = new ConnectingData(tcpClient, Sender, connect.ServerInfo, connect.ClientName);

                        logger.LogTrace("Moving to Connecting state");
                        return GoTo(MainState.Connecting).Using(stateData);
                    }
                    catch
                    {
                        Self.Tell(connect);
                    }
                }

                return null;
            });
        }
    }
}
