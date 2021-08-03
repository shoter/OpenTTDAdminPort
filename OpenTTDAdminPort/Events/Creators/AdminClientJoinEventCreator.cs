
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientInfoEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO;

        public IAdminEvent? Create(in IAdminMessage message, in IAdminPortClientContext context)
        {
            var msg = (AdminServerClientInfoMessage)message;
            var player = context.Players[msg.ClientId];

            return new AdminClientInfoEvent(player, msg.JoinDate, msg.PlayingAs, msg.Hostname);
        }
    }
}
