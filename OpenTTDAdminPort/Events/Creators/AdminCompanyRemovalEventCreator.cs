using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    public class AdminCompanyRemovalEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData prev, in ConnectedData data)
        {
            var msg = (AdminServerCompanyInfoMessage)message;
            var company = data.Companies[msg.CompanyId];

            return new AdminCompanyRemovalEvent(company);
        }
    }
}