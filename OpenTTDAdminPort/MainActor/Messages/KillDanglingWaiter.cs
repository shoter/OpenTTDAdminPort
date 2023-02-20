using Akka.Actor;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record KillDanglingWaiter(IActorRef Waiter);
}
