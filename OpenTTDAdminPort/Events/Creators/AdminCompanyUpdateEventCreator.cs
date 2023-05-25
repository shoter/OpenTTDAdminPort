using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    public class AdminCompanyUpdateEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData prev, in ConnectedData data)
        {
            var msg = (AdminServerCompanyUpdateMessage)message;
            var prevCompany = prev.Companies[msg.CompanyId];
            var company = data.Companies[msg.CompanyId];

            return new AdminCompanyUpdateEvent(prevCompany, company);
        }
    }
}