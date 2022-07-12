using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public static class DockerClientExtensions
    {
        public static async Task RemoveContainerIfExists(this DockerClient client, string containerName)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 1000 });
            string containerId = null;

            foreach (var c in containers)
            {
                foreach (var name in c.Names)
                {
                    if (name.TrimStart('/') == containerName)
                    {
                        containerId = c.ID;
                        break;
                    }
                }

                if (containerId != null)
                {
                    break;
                }
            }

            if (containerId != null)
            {
                await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters() { });
                await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true });
            }
        }

        public static async Task StopRemoveContainer(this DockerClient client, string containerName)
        {
            await client.Containers.StopContainerAsync(containerName, new ContainerStopParameters());
            await client.Containers.RemoveContainerAsync(containerName,
                new ContainerRemoveParameters { Force = true, RemoveVolumes = true });
        }

        public static async Task StopContainer(this DockerClient client, string containerName)
        {
            await client.Containers.StopContainerAsync(containerName, new ContainerStopParameters() { });
        }

        public static async Task ResumeContainer(this DockerClient client, string containerName)
        {
            await client.Containers.UnpauseContainerAsync(containerName);
        }

        public static async Task PullImage(this DockerClient client, string imageName, string tagName)
        {
            var pullParam = new ImagesCreateParameters()
            {
                FromImage = imageName,
                Tag = tagName,
            };

            await client.Images.CreateImageAsync(pullParam, new AuthConfig(), new DockerizePullProgress(imageName));
        }
    }
}
