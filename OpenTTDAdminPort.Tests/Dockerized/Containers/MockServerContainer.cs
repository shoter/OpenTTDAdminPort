using Docker.DotNet;
using Docker.DotNet.Models;

using OpenTTDAdminPort.Networking;

using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Dockerized.Containers
{
    public class MockServerContainer : ContainerApplication
    {

        public MockServerContainer(DockerClient client) : base(client)
        {
        }

        protected override string ImageName => "mockserver/mockserver";

        protected override string TagName => "latest";

        protected override CreateContainerParameters OverrideContainerParameters(CreateContainerParameters options, int assignedPort)
        {
            options.AddPortBinding(Port, 1080);

            return options;
        }

        protected override async Task WaitForContainerStart()
        {
            while (true)
            {
                try
                {
                    using ITcpClient client = new MyTcpClient();
                    await client.ConnectAsync("127.0.0.1", Port);
                    break;
                }
                catch { }
            }
        }
    }
}
