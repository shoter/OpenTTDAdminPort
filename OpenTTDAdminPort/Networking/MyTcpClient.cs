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
        private readonly TcpClient client;
        public EndPoint RemoteEndPoint => client.Client.RemoteEndPoint;
        public Stream GetStream() => client.GetStream();

        public MyTcpClient()
        {
            client = new TcpClient();
        }

        public MyTcpClient(TcpClient client)
        {
            this.client = client;
        }

        public Task ConnectAsync(string ip, int port)
        {
            return client.ConnectAsync(IPAddress.Parse(ip), port);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public void Close()
        {
            if (_client.Connected)
            {
                _client.GetStream().Close();
                _client.Close();
            }
        }
    }
}
