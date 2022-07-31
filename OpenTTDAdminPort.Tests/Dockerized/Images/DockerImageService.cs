using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

using OpenTTDAdminPort.Tests.Dockerized.Progress;

namespace OpenTTDAdminPort.Tests.Dockerized.Images
{
    public class DockerImageService : IDockerImageService
    {
        private readonly IDockerClient dockerClient;

        private readonly IDockerProgressFactory dockerProgressFactory;

        public DockerImageService(IDockerClient dockerClient, IDockerProgressFactory dockerProgressFactory)
        {
            this.dockerClient = dockerClient;
            this.dockerProgressFactory = dockerProgressFactory;
        }

        public Task RemoveIfExistDockerImage(string imageName, string tagName, CancellationToken token = default)
        {
            if (SearchForLocalImage(imageName, tagName, token) == null)
            {
                return Task.CompletedTask;
            }

            string name = $"{imageName}:{tagName}";

            // https://docs.docker.com/engine/api/v1.41/#operation/ImageDelete
            return dockerClient.Images.DeleteImageAsync(name, new ImageDeleteParameters()
            {
                Force = true,
            }, token);
        }

        /// <inheritdoc/>
        public async Task<ImagesListResponse> SearchForLocalImage(string imageName, string tagName, CancellationToken token = default)
        {
            string name = $"{imageName}:{tagName}";

            // https://docs.docker.com/engine/api/v1.41/#operation/ImageList
            var results = await dockerClient.Images.ListImagesAsync(new ImagesListParameters()
            {
                // This is a montrosity. This is insane
                Filters = new Dictionary<string, IDictionary<string, bool>>()
                {
                    {
                        "reference",  new Dictionary<string, bool>() { { name, true } }
                    },
                },
            }, token);

            return results
                .SingleOrDefault();
        }

        public Task PullImage(string imageName, string tagName, CancellationToken token = default)
        {
            var pullParam = new ImagesCreateParameters()
            {
                FromImage = imageName,
                Tag = tagName,
            };

            // https://docs.docker.com/engine/api/v1.41/#operation/ImageCreate
            return this.dockerClient.Images.CreateImageAsync(pullParam, new AuthConfig(), dockerProgressFactory.Create(), token);
        }
    }
}
