using OpenTTDAdminPort.MainActor.StateData;

using System;

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
