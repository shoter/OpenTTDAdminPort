using System;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Events;

namespace OpenTTDAdminPort
{
    public class AdminPortClientMessager : ReceiveActor
    {
        private Action<IAdminEvent>? adminEventOnReceive;
        private ILogger logger;

        private IServiceScope scope;

        private IServiceProvider Sp => scope.ServiceProvider;

        public AdminPortClientMessager(IServiceProvider serviceProvider)
        {
            this.scope = serviceProvider.CreateScope();
            this.logger = Sp.GetRequiredService<ILogger<AdminPortClientMessager>>();
            Ready();
        }

        protected override void PostStop()
        {
            scope.Dispose();
            base.PostStop();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new AdminPortClientMessager(sp));

        public void Ready()
        {
            Receive<Action<IAdminEvent>>(SetNewAction);
            Receive<IAdminEvent>(e =>
            {
                adminEventOnReceive?.Invoke(e);
            });
        }

        public void SetNewAction(Action<IAdminEvent> ev)
        {
            adminEventOnReceive = ev;
            Sender.Tell(SuccessResponse.Instance);
        }
    }
}
