
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
        public async Task ConnectToMockServerMultipleTimes_WithoutProblems()
        {
            await serverContainer.Start(nameof(ConnectToMockServerMultipleTimes_WithoutProblems));

            for(int i = 0;i < 50; ++i)
            {
                using MyTcpClient client = new MyTcpClient();
                await client.ConnectAsync("127.0.0.1", serverContainer.Port);
                client.Close();
            }
        }
    }
}
