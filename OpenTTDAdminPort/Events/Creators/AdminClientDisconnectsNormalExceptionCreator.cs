using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientDisconnectsNormalExceptionCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData prev, in ConnectedData data)
        {
            var msg = (AdminServerClientErrorMessage)message;

            if (!prev.Players.ContainsKey(msg.ClientId))
            {
                return null;
            }

            var player = prev.Players[msg.ClientId];

            return new AdminClientDisconnectEvent(player, "error");
        }
    }
}
