using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminCompanyRemovalEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.CompanyInfo;

        public Company Company { get; }

        public AdminCompanyRemovalEvent(Company company)
        {
            Company = company;
        }
    }
}