using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminCompanyInfoEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.CompanyInfo;

        public Company Company { get; }

        public AdminCompanyInfoEvent(Company company)
        {
            Company = company;
        }
    }
}