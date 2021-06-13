using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public abstract class ContainerApplication : IContainerApplication
    {
        public int Port { get; private set; }

        protected abstract string ImageName { get; }

        protected abstract string TagName { get; }

        private string containerName;

        private ContainerApplicationState State { get; set; } = ContainerApplicationState.Idle;

        private readonly DockerClient client = DockerClientProvider.Instance;


        public ContainerApplication(DockerClient client)
        {
            this.client = client;
        }

        public async Task Start(string containerName)
        {
            this.containerName = containerName;

            await client.RemoveContainerIfExists(containerName);
            // Image might not exist on local pc. We need to download it.
            await client.PullImage(ImageName, TagName);

            Port = GetFreeTcpPort();
            var parameters = OverrideContainerParameters(new CreateContainerParameters()
            {
                Name = containerName,
                Image = $"{ImageName}:{TagName}",
            }, Port);


            var response = await client.Containers.CreateContainerAsync(parameters);

            await client.Networks.ConnectNetworkAsync("bridge", new NetworkConnectParameters
            {
                Container = response.ID,
            });

            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters() { });

            await WaitForContainerStart();

            State = ContainerApplicationState.Running;
        }

        public async Task Stop()
        {
            if (State == ContainerApplicationState.Running)
            {
                await client.StopRemoveContainer(containerName);
                State = ContainerApplicationState.Stopped;
            }
        }

        protected virtual CreateContainerParameters OverrideContainerParameters(CreateContainerParameters options, int assignedPort) => options;

        protected abstract Task WaitForContainerStart();


        public void Dispose()
        {
            if (State == ContainerApplicationState.Running)
            {
                Stop().Wait();
            }
        }

        protected static int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }


    }
}
