using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerCompanyUpdateMessageProcessor : SingleMessageProcessorBase<AdminServerCompanyUpdateMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerCompanyUpdateMessage message)
        {
            if (!data.Companies.ContainsKey(message.CompanyId))
            {
                return data;
            }

            Company company = data.Companies[message.CompanyId] with
            {
                Name = message.CompanyName,
                ManagerName = message.ManagerName,
                Color = message.Color,
                HasPassword = message.HasPassword,
                MonthsOfBankruptcy = message.MonthsOfBankruptcy,
            };

            return data.UpsertCompany(company);
        }
    }
}