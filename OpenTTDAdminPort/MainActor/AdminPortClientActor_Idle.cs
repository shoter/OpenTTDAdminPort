using Akka.Actor;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>, IWithUnboundedStash
    {

        public void IdleState()
        {
            When(MainState.Idle, state =>
            {
                if (state.FsmEvent is AdminPortConnect connect)
                {
                    logger.LogTrace($"Received connect message to {connect.ServerInfo}");

                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, connect.ServerInfo.ServerIp, connect.ServerInfo.ServerPort);
                    var stateData = new ConnectingData(tcpClient, connect.ServerInfo, connect.ClientName);

                    logger.LogTrace("Moving to Connecting state");
                    return GoTo(MainState.Connecting).Using(stateData);
                }

                return null;
            });
        }

    }
}
