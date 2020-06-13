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

        public ServerInfo Server { get; }
        public AdminChatMessageEvent(Player player, string msg, ServerInfo info)
        {
            this.Player = player;
            this.Message = msg;
            this.Server = info;
        }
    }
}
