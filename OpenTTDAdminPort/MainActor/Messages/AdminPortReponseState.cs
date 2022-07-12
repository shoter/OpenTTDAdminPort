using System;

using OpenTTDAdminPort.MainActor.StateData;

namespace OpenTTDAdminPort.MainActor.Messages
{
    public class AdminPortReponseState
    {
        public Guid QueryId { get; }

        public MainActorState State { get; }

        public AdminPortReponseState(AdminPortQueryState query, MainActorState state)
        {
            this.QueryId = query.Id;
            this.State = state;
        }
    }
}
