using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messaging
{
    public class AdminPollMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_POLL;

        public AdminUpdateType UpdateType { get; }

        public uint Argument { get; }

        public AdminPollMessage(AdminUpdateType updateType, uint argument)
        {
            this.UpdateType = updateType;
            this.Argument = argument;
        }
    }
}
