using OpenTTDAdminPort.Game;
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
            Player player = new(msg.ClientId, "unknown", "unknown", 255, default);
            if (prev.Players.ContainsKey(msg.ClientId))
            {
                player = prev.Players[msg.ClientId];
            }

            return new AdminClientDisconnectEvent(player, "normal");
        }
    }
}
