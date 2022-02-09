using Akka.Actor;

using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.MainActor.StateData;

namespace OpenTTDAdminPort.MainActor
{
    public partial class AdminPortClientActor : FSM<MainState, IMainData>
    {

        public void IdleState()
        {
            When(MainState.Idle, state =>
            {
                if (state.FsmEvent is AdminPortConnect connect)
                {
                    IActorRef tcpClient = actorFactory.CreateTcpClient(Context, connect.ServerInfo.ServerIp, connect.ServerInfo.ServerPort);
                    var stateData = new ConnectingData(tcpClient, connect.ServerInfo, connect.ClientName);

                    return GoTo(MainState.Connecting).Using(stateData);
                }

                return null;
            });
        }

    }
}
