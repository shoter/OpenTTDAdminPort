using System;
using System.Threading.Tasks;

using Akka.Actor;

namespace OpenTTDAdminPort.Akkas
{
    public static class ActorExtensions
    {
        public static async Task<object> TryAsk(this IActorRef actor, object message)
        {
            object response = await actor.Ask(message);

            if (response is Exception ex)
            {
                throw ex;
            }

            return response;
        }
    }
}
