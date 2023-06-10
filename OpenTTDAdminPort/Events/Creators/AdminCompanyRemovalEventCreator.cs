using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    public class AdminCompanyRemovalEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData prev, in ConnectedData data)
        {
            var msg = (AdminServerCompanyRemoveMessage)message;

            // Some defensive programming
            // There is always chance that we might lose message about company creation etc.
            if (!prev.Companies.ContainsKey(msg.CompanyId))
            {
                return null;
            }

            var company = prev.Companies[msg.CompanyId];

            return new AdminCompanyRemovalEvent(company);
        }
    }
}