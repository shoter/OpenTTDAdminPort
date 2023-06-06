using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal interface ISingleMessageProcessor
    {
        ConnectedData ProcessAdminMessage(
            ConnectedData data,
            IAdminMessage message);
    }
}