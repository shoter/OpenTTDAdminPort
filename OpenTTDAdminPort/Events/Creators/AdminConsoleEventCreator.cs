using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminConsoleEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData context)
        {
            var msg = (AdminServerConsoleMessage)message;

            return new AdminConsoleEvent(msg.Origin, msg.Message);
        }
    }
}
