using System.IO;
using System.Threading.Tasks;

using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class DummyTcpClient : ITcpClient
    {
        public MemoryStream Stream { get; } = new();

        public bool IsConnected { get; set; } = false;

        public string Ip { get; private set; }

        public int Port { get; private set; }

        public void Close()
        {
            IsConnected = false;
        }

        public Task ConnectAsync(string ip, int port)
        {
            IsConnected = true;
            this.Ip = ip;
            this.Port = port;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        public Stream GetStream() => Stream;
    }
}
