using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal interface ITcpClient
    {
        Stream GetStream();
        EndPoint RemoteEndPoint { get; }
        Task ConnectAsync(string ip, int port);

    }
}
