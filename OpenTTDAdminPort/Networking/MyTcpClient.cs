using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace OpenTTDAdminPort.Networking
{
    public class MyTcpClient : ITcpClient, IDisposable
    {
        // The real implementation
        private readonly TcpClient _client;
        public EndPoint RemoteEndPoint => _client.Client.RemoteEndPoint;
        public Stream GetStream() => _client.GetStream();

        public MyTcpClient()
        {
            _client = new TcpClient();
        }

        public MyTcpClient(TcpClient client)
        {
            _client = client;
        }

        public Task ConnectAsync(string ip, int port)
        {
            return _client.ConnectAsync(IPAddress.Parse(ip), port);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void Close()
        {
            _client.GetStream().Close();
            _client.Close();
        }
    }
}
