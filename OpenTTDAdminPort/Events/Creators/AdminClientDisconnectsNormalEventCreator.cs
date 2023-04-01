using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    internal class AdminClientDisconnectsNormalEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData prev, in ConnectedData data)
        {
            var msg = (AdminServerClientQuitMessage)message;

            if (!prev.Players.ContainsKey(msg.ClientId))
            {
                return null;
            }

            var player = prev.Players[msg.ClientId];

            return new AdminClientDisconnectEvent(player, "normal");
        }
    }
}
