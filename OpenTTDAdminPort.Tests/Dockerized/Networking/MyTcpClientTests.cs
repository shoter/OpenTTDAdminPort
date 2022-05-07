
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Tests.Dockerized.Containers;

using System;
using System.Threading.Tasks;

using Xunit;

namespace OpenTTDAdminPort.Tests.Dockerized.Networking
{
    public class MyTcpClientTests
    {
        MockServerContainer serverContainer = new MockServerContainer(DockerClientProvider.Instance);

        [Fact]
        public async Task ConnectToMockServer()
        {
            await serverContainer.Start(nameof(ConnectToMockServer));
            using MyTcpClient client = new MyTcpClient();
            await client.ConnectAsync("127.0.0.1", serverContainer.Port);
            client.Close();
        }
    }
}
