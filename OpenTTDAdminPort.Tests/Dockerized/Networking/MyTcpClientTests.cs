using System;
using System.Threading.Tasks;

using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Tests.Dockerized.Applications;
using OpenTTDAdminPort.Tests.Dockerized.Containers;

using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Dockerized.Networking
{
    public class MyTcpClientTests : DockerizedTest<MockServerContainer>
    {
        public MyTcpClientTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        [Fact]
        public async Task ConnectToMockServer()
        {
            await application.Start();
            using MyTcpClient client = new MyTcpClient();
            await client.ConnectAsync("127.0.0.1", application.Port);
            client.Close();
        }
    }
}
