using Akka.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Akkas
{
    public static class ActorExtensions
    {
        public static async Task<object> TryAsk(this IActorRef actor, object message)
        {
            object response = await actor.Ask(message);

            if(response is Exception ex)
            {
                throw ex;
            }

            return response;
        }

    }
}
