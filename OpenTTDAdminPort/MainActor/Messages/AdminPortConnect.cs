namespace OpenTTDAdminPort.MainActor.Messages
{
    public class AdminPortConnect
    {
        public ServerInfo ServerInfo { get; set; }

        public string ClientName { get; set; }

        public AdminPortConnect(ServerInfo serverInfo, string clientName)
        {
            this.ServerInfo = serverInfo;
            this.ClientName = clientName;
        }
    }
}
