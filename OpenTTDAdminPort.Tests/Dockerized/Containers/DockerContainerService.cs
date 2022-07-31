using System.Threading;
using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized.Containers
{
    public class DockerContainerService : IDockerContainerService
    {
        private readonly IDockerClient client;

        public DockerContainerService(IDockerClient dockerClient) => this.client = dockerClient;

        public async Task RemoveContainer(string containerNameOrId, CancellationToken token = default)
        {
            try
            {
                await this.client.Containers.RemoveContainerAsync(containerNameOrId,
                    new ContainerRemoveParameters() { Force = true, RemoveVolumes = true }, token);
            }
            catch (DockerContainerNotFoundException)
            {
                // If container is not there then it is fine to just ignore not found exception.
                // Exception-controlled flow of application is not nice, however there is no other way to that.
            }
        }

        public async Task StopContainer(string containerNameOrId, CancellationToken token = default)
        {
            try
            {
                await this.client.Containers.StopContainerAsync(containerNameOrId, new ContainerStopParameters() { }, token);
            }
            catch (DockerContainerNotFoundException)
            {
                // Refer to comment in RemoveContainer
            }
        }
    }
}
