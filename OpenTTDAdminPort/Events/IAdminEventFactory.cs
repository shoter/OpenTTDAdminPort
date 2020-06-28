using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;

namespace OpenTTDAdminPort.Events
{
    internal interface IAdminEventFactory
    {
        IAdminEvent? Create(in IAdminMessage adminMessage, in IAdminPortClientContext context);
    }
}
