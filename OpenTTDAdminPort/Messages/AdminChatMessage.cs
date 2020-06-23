using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    public class AdminChatMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_CHAT;

        public NetworkAction NetworkAction { get; }

        public ChatDestination ChatDestination { get; }

        public uint Destination { get; }

        public string Message { get; }

        public AdminChatMessage(NetworkAction networkAction, ChatDestination chatDestination, uint destination, string message)
        {
            this.NetworkAction = networkAction;
            this.ChatDestination = chatDestination;
            this.Message = message;
            this.Destination = destination;
        }
    }
}
