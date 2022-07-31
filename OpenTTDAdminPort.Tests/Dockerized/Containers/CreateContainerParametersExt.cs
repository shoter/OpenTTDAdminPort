using System.Collections.Generic;
using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized.Containers
{
    public class CreateContainerParametersExt : CreateContainerParameters
    {
        /// <summary>
        /// Adds host to /etc/hosts.
        /// after execution of this command <paramref name="host"/> is going to point to <paramref name="destinationAddress"/> address.
        /// </summary>
        /// <param name="host">Source address</param>
        /// <param name="destinationAddress">Address to which source address is going to be pointed</param>
        public void AddHost(string host, string destinationAddress)
        {
            this.HostConfig.ExtraHosts = this.HostConfig.ExtraHosts ?? new List<string>();
            this.HostConfig.ExtraHosts.Add($"{host}:{destinationAddress}");
        }

        public void AddPortBinding(int hostPort, int containerPort)
        {
            this.HostConfig = this.HostConfig ?? new HostConfig();
            this.HostConfig.PortBindings = this.HostConfig.PortBindings ?? new Dictionary<string, IList<PortBinding>>();

            this.HostConfig.PortBindings.Add($"{containerPort}/tcp", new List<PortBinding>()
            {
                new PortBinding
                {
                    HostPort = hostPort.ToString(),
                },
            });

            this.ExposedPorts = this.ExposedPorts ?? new Dictionary<string, EmptyStruct>();
            this.ExposedPorts.Add($"{containerPort}/tcp", default(EmptyStruct));
        }

        public void AddVolumeBind(string hostPath, string containerPath)
        {
            string bind = $"{hostPath}:{containerPath}";

            this.HostConfig = this.HostConfig ?? new HostConfig();
            this.HostConfig.Binds = this.HostConfig.Binds ?? new List<string>();
            this.HostConfig.Binds.Add(bind);
        }
    }
}
