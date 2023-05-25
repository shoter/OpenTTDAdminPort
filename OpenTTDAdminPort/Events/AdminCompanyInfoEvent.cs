using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public record AdminCompanyInfoEvent(Company Company) : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.CompanyNew;
    }
}