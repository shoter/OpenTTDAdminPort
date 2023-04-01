using Newtonsoft.Json;

namespace OpenTTDAdminPort
{
    public class ServerInfo
    {
        public string ServerIp { get; }

        public int ServerPort { get; }

        [JsonIgnore]
        public string Password { get; }

        public ServerInfo(string serverIp, int serverPort, string password = "")
        {
            this.ServerIp = serverIp;
            this.ServerPort = serverPort;
            this.Password = password;
        }

        public override string ToString() => $"{ServerIp}:{ServerPort}";
    }
}
