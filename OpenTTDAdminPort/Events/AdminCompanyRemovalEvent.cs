using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public record AdminCompanyRemovalEvent(Company Company) : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.CompanyRemoval;
    }
}