using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerCompanyInfoMessageProcessor : SingleMessageProcessorBase<AdminServerCompanyInfoMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerCompanyInfoMessage message)
        {
            Company company = new Company(
                message.CompanyId,
                message.CompanyName,
                message.ManagerName,
                message.Color,
                message.HasPassword,
                message.CreationDate,
                message.IsAi,
                message.MonthsOfBankruptcy);

            return data.UpsertCompany(company);
        }
    }
}