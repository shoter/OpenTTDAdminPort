using System;
using OpenTTDAdminPort.Events;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record WaitForEvent(Func<IAdminEvent, bool> WaiterFunc);
}
