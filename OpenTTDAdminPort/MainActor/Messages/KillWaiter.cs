using System;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public record KillWaiter(Guid RequestId);
}