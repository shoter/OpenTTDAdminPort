using System;
using System.Threading;
using OpenTTDAdminPort.Events;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record WaitForEvent(Func<IAdminEvent, bool> WaiterFunc, CancellationToken Token);
}
