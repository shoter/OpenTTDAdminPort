using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messages
{
    public record AdminServerChatMessage(
        NetworkAction NetworkAction,
        ChatDestination ChatDestination,
        uint ClientId,
        string Message,
        long Data) : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CHAT;
    }
}
