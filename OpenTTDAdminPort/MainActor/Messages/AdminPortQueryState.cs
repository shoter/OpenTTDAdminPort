using System;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public class AdminPortQueryState
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
