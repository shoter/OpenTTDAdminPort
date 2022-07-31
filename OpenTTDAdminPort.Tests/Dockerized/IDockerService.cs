using Docker.DotNet;

using OpenTTDAdminPort.Tests.Dockerized.Containers;
using OpenTTDAdminPort.Tests.Dockerized.Images;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public interface IDockerService
    {
        public IDockerImageService Images { get; }

        public IDockerContainerService Containers { get; }

        public IDockerClient Client { get; }
    }
}
