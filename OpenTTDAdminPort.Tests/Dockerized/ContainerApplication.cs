using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

using DotNet.Testcontainers.Builders;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Tests.Dockerized.Containers;

using Polly;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public abstract class ContainerApplication : IContainerApplication
    {
        public int Port { get; private set; }

        protected abstract string ImageName { get; }

        protected abstract string TagName { get; }

        private string containerName;

        public ContainerApplicationState State { get; private set; } = ContainerApplicationState.Idle;

        protected readonly ILogger logger;

        protected readonly IDockerService docker;

        protected AsyncPolicy<bool> containerStartPolicy;

        public ContainerApplication(IDockerService dockerService, ILogger logger)
        {
            this.logger = logger;
            this.docker = dockerService;
            containerStartPolicy = Policy
                .HandleResult<bool>(x => x == false)
                .Or<Exception>()
                .WaitAndRetryAsync(60, retryAttempt => TimeSpan.FromSeconds(1));
        }

        public async virtual Task Start([CallerMemberName] string containerName = null)
        {
            if (containerName == null)
            {
                throw new ArgumentNullException(containerName);
            }

            this.containerName = containerName;

            await docker.Containers.StopAndRemoveContainer(containerName);

            // Image might not exist on local pc. We need to download it.
            await docker.Images.PullImage(ImageName, TagName);

            Port = GetFreeTcpPort();
            var parameters = OverrideContainerParameters(new CreateContainerParametersExt()
            {
                Name = containerName,
                Image = $"{ImageName}:{TagName}",
            });

            var response = await docker.Client.Containers.CreateContainerAsync(parameters);

            await docker.Client.Networks.ConnectNetworkAsync("bridge", new NetworkConnectParameters
            {
                Container = response.ID,
            });

            await docker.Client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters() { });

            await containerStartPolicy.ExecuteAndCaptureAsync(CheckIfContainerIsRunning);
            State = ContainerApplicationState.Running;
        }

        public async Task Stop()
        {
            try
            {
                if (State == ContainerApplicationState.Running)
                {
                    await docker.Containers.StopAndRemoveContainer(containerName);
                    State = ContainerApplicationState.Stopped;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ocurred while stopping container");
                State = ContainerApplicationState.Errored;
                throw;
            }
        }

        protected virtual CreateContainerParameters OverrideContainerParameters(CreateContainerParametersExt options) => options;

        protected abstract Task<bool> CheckIfContainerIsRunning();

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
