using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public record AdminCompanyUpdateEvent(Company PreviousCompany, Company Company) : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.CompanyUpdate;
    }
}