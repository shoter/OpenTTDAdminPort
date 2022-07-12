using System;

using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events.Creators
{
    public class AdminChatMessageEventCreator : IEventCreator
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_CHAT;

        public IAdminEvent? Create(in IAdminMessage message, in ConnectedData data)
        {
            var chat = message as AdminServerChatMessage;
            ArgumentNullException.ThrowIfNull(chat);
            var player = data.Players[chat.ClientId];
            return new AdminChatMessageEvent(player, chat.ChatDestination, chat.NetworkAction, chat.Message);
        }
    }
}
