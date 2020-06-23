namespace OpenTTDAdminPort.Messaging
{
    public class AdminServerGamescriptMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_SERVER_GAMESCRIPT;

        public string Json { get; }

        public AdminServerGamescriptMessage(string json)
        {
            this.Json = json;
        }


    }
}
