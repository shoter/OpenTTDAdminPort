using Akka.Actor;

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
            Receive<Action<IAdminEvent>>(a => adminEventOnReceive = a);
            Receive<IAdminEvent>(e => adminEventOnReceive?.Invoke(e));
        }
    }
}
