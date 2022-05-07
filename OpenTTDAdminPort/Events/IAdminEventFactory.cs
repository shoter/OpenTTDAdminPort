using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events
{
    internal interface IAdminEventFactory
    {
        IAdminEvent? Create(in IAdminMessage adminMessage, in ConnectedData context);
    }
}
