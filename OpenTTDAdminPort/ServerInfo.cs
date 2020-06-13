using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class ServerInfo
    {
        public string ServerIp { get; }

        public int ServerPort { get; }

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
