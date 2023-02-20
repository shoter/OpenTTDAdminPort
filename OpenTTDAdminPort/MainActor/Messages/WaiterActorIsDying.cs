using Akka.Actor;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record WaiterActorIsDying(IActorRef Actor);
}
