using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Tests.Dockerized.Containers;

namespace OpenTTDAdminPort.Tests.Dockerized.Applications
{
    public class MockServerContainer : ContainerApplication
    {
        public MockServerContainer(IDockerService dockerService, ILogger<MockServerContainer> logger)
            : base(dockerService, logger)
        {
        }

        protected override string ImageName => "mockserver/mockserver";

        protected override string TagName => "latest";

        protected override CreateContainerParameters OverrideContainerParameters(CreateContainerParametersExt options)
        {
            options.AddPortBinding(Port, 1080);

            return options;
        }

        protected override async Task<bool> CheckIfContainerIsRunning()
        {
            using ITcpClient client = new MyTcpClient();
            await client.ConnectAsync("127.0.0.1", Port);
            return true;
        }
    }
}
