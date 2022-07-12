using System;
using System.Runtime.InteropServices;

using Docker.DotNet;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public static class DockerClientProvider
    {
        public static DockerClient Instance { get; }

        static DockerClientProvider()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Instance = new DockerClientConfiguration(
                new Uri("unix:/var/run/docker.sock"))
                .CreateClient();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Instance = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();
            }
            else
            {
                throw new NotSupportedException("This os is not supported");
            }
        }
    }
}
