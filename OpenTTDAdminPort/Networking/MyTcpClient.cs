using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    public class MyTcpClient : ITcpClient, IDisposable
    {
        // The real implementation
        private TcpClient Client { get; } = new();

        public Stream GetStream() => Client.GetStream();

        public Task ConnectAsync(string ip, int port)
        {
            return Client.ConnectAsync(IPAddress.Parse(ip), port);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        public void Close()
        {
            if (Client.Connected)
            {
                Client.GetStream().Close();
                Client.Close();
            }
        }
    }
}
