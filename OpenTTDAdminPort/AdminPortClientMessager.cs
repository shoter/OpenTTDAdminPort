using Akka.Actor;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;

using System;

namespace OpenTTDAdminPort
{
    public class AdminPortClientMessager : ReceiveActor
    {
        private Action<IAdminEvent>? adminEventOnReceive;
        public AdminPortClientMessager()
        {
            Ready();
        }

        public static Props Create()
            => Props.Create(() => new AdminPortClientMessager());

        public void Ready()
        {
            Receive<Action<IAdminEvent>>(SetNewAction);
            Receive<IAdminEvent>(e => adminEventOnReceive?.Invoke(e));
        }

        public void SetNewAction(Action<IAdminEvent> ev)
        {
            adminEventOnReceive = ev;
            Sender.Tell(SuccessResponse.Instance);
        }
    }
}
