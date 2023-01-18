using System;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Akkas;

namespace OpenTTDAdminPort
{
    internal class AdminPortClientMessager : ScopedReceiveActor
    {
        private Action<object>? adminMessageOnReceive;
        private ILogger logger;

        public AdminPortClientMessager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.logger = SP.GetRequiredService<ILogger<AdminPortClientMessager>>();
            Ready();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new AdminPortClientMessager(sp));

        public void Ready()
        {
            Receive<Action<object>>(SetNewAction);
            Receive<object>(e =>
            {
                adminMessageOnReceive?.Invoke(e);
            });
        }

        public void SetNewAction(Action<object> action)
        {
            adminMessageOnReceive = action;
            Sender.Tell(SuccessResponse.Instance);
        }
    }
}
