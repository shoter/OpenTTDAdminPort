using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;

namespace OpenTTDAdminPort.Events
{
    internal interface IAdminMessageProcessor
    {
        IAdminEvent? ProcessMessage(in IAdminMessage adminMessage, in IAdminPortClientContext context);
    }
}
