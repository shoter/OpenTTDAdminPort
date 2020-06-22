using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Messaging
{
    internal class AdminServerClientErrorMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR;

        public uint ClientId { get; }

        public NetworkErrorCode Error { get; }

        public AdminServerClientErrorMessage(uint clientId, byte error)
        {
            this.ClientId = clientId;
            this.Error = (NetworkErrorCode)error;
        }
    }
}
