using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerDateMessageProcessor : SingleMessageProcessorBase<AdminServerDateMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerDateMessage message)
        {
            return data with
            {
                AdminServerInfo = data.AdminServerInfo with
                {
                    Date = message.Date,
                },
            };
        }
    }
}