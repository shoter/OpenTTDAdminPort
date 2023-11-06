using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public record AdminChatMessageEvent(
        Player Player,
        ChatDestination ChatDestination,
        NetworkAction NetworkAction,
        string Message) : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ChatMessageReceived;
    }
}
