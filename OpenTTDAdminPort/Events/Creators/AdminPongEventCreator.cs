using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminPongEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_PONG;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData context)
        {
            var msg = (AdminServerPongMessage)message;

            return new AdminPongEvent(msg.Argument);
        }
    }
}
