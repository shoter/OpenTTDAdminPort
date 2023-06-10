using System;
using System.Threading;
using Akka.Actor;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record KillDanglingWaiter(Guid WaiterId, CancellationToken Token);
}
