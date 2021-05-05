using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class DockerizedTest
    {
        public int Port { get; private set; }
        public ServerInfo ServerInfo => new ServerInfo("127.0.0.1", Port, "admin_pass");
        private readonly DockerClient client;

        private string containerName;

        public DockerizedTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                client = new DockerClientConfiguration(
                new Uri("unix:/var/run/docker.sock"))
                .CreateClient();

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                client = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();
            }
            else
            {
                throw new NotSupportedException("This os is not supported");
            }
        }

        public void Dispose()
        {
            cleanUp();
        }

        private void cleanUp()
        {
            if (client != null)
            {
                client.Containers.StopContainerAsync(containerName, new ContainerStopParameters()).Wait();
                client.Containers.RemoveContainerAsync(containerName,
                    new ContainerRemoveParameters { Force = true, RemoveVolumes = true }).Wait();
                client.Dispose();
            }
        }

        private async Task RemoveContainerIfExists(string containerName)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 1000 });
            string containerId = null;

            foreach (var c in containers)
            {
                foreach (var name in c.Names)
                    if (name.TrimStart('/') == containerName)
                    {
                        containerId = c.ID;
                        break;
                    }
                if (containerId != null)
                    break;
            }

            if (containerId != null)
            {
                await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters() { });
                await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true });
            }
        }


        public async Task Start(string containerName)
        {
            // https://github.com/dotnet/Docker.DotNet/issues/402 - There is some nice info in this issue with nice real life example.
            this.containerName = containerName;

            await RemoveContainerIfExists(containerName);
            await StartContainer(containerName);
            await WaitForServerToStart();
        }

        private async Task WaitForServerToStart()
        {
            AdminPortClient client = null;
            while (client?.ConnectionState != AdminConnectionState.Connected)
            {
                try
                {
                    client = new AdminPortClient(new ServerInfo("127.0.0.1", Port, "admin_pass"));
                    await client.Connect();
                }

                catch (Exception) { /* swallow the exception */ }
            }

        }

        protected async Task StartTest(string containerName, Func<Task> test)
        {
            await Start(containerName);
            try
            {
                await test();
            }
            finally
            {
                cleanUp();
            }

        }

        private async Task StartContainer(string containerName)
        {
            this.Port = GetFreeTcpPort();
            string image = "redditopenttd/openttd";
            string tag = "1.11.2";
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), nameof(Dockerized), "openttd.cfg");

            var pullParam = new ImagesCreateParameters()
            {
                FromImage = image,
                Tag = tag
            };

            await client.Images.CreateImageAsync(pullParam, new AuthConfig(), new DockerizePullProgress(image));

            var response = await client.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters()
            {
                Name = containerName,
                Image = $"{image}:{tag}",
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "3982/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = Port.ToString()
                                }
                            }
                        }
                    },
                    Binds = new List<string>
                    {
                        $"{configPath}:/config/openttd.cfg:ro"
                    },
                },

                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "3982/tcp", new EmptyStruct(){ }
                    }
                }
            }); ;

            await client.Networks.ConnectNetworkAsync("bridge", new NetworkConnectParameters
            {
                Container = response.ID,
            });

            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters() { });
        }

        private static int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }



    }
}
