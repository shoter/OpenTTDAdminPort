using OpenTTDAdminPort.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Events
{
    public class AdminChatMessageEvent : IAdminEvent
    {
        public AdminEventType EventType => AdminEventType.ChatMessageReceived;


        public Player Player { get; }
        public string Message { get; }
        public ChatDestination ChatDestination { get; }
        public NetworkAction NetworkAction { get; }

        public AdminChatMessageEvent(Player player, ChatDestination chatDestination, NetworkAction action, string msg)
        {
            this.Player = player;
            this.Message = msg;
            this.ChatDestination = chatDestination;
            this.NetworkAction = action;
        }
    }
}
