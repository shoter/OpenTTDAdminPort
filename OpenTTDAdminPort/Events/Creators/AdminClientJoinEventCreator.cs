using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientInfoEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData context)
        {
            var msg = (AdminServerClientInfoMessage)message;
            var player = context.Players[msg.ClientId];

            return new AdminClientInfoEvent(player, msg.JoinDate);
        }
    }
}
