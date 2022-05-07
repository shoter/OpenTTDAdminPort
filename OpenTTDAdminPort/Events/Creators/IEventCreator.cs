using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal interface IEventCreator
    {
        AdminMessageType SupportedMessageType { get; }

        IAdminEvent? Create(in IAdminMessage message, in ConnectedData data);
    }
}
