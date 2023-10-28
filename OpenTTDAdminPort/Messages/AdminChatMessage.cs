using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    public record AdminChatMessage(
        NetworkAction NetworkAction,
        ChatDestination ChatDestination,
        uint Destination,
        string Message) : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_CHAT;

        public override string ToString() =>
            $"{MessageType} - {NetworkAction} - {ChatDestination} - {Destination} - {Message}";
    }
}