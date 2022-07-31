using Docker.DotNet;

using OpenTTDAdminPort.Tests.Dockerized.Containers;
using OpenTTDAdminPort.Tests.Dockerized.Images;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class DockerService : IDockerService
    {
        public IDockerImageService Images { get; }

        public IDockerContainerService Containers { get; }

        public IDockerClient Client { get; }

        public DockerService(IDockerImageService images, IDockerContainerService containers, IDockerClient client)
        {
            this.Images = images;
            this.Containers = containers;
            this.Client = client;
        }
    }
}
