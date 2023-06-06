using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor
{
    public interface IIncomingMessageProcessor
    {
        ConnectedData ProcessAdminMessage(
            ConnectedData initial,
            IAdminMessage message);
    }
}