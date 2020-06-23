using System.Collections.Generic;

namespace OpenTTDAdminPort.Messaging
{
    public class AdminServerCmdNamesMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES;

        public Dictionary<ushort, string> Commands { get; }

        public AdminServerCmdNamesMessage(Dictionary<ushort, string> commands)
        {
            this.Commands = commands;
        }
    }
}
