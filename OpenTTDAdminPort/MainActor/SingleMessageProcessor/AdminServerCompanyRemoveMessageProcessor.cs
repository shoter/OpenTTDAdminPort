using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerCompanyRemoveMessageProcessor : SingleMessageProcessorBase<AdminServerCompanyRemoveMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerCompanyRemoveMessage message)
        {
            return data.RemoveCompany(message.CompanyId);
        }
    }
}