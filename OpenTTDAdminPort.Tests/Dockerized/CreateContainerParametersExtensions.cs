using System.Collections.Generic;

using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public static class CreateContainerParametersExtensions
    {
        public static void AddPortBinding(this CreateContainerParameters parameters, int hostPort, int containerPort)
        {
            parameters.HostConfig = parameters.HostConfig ?? new HostConfig();
            parameters.HostConfig.PortBindings = parameters.HostConfig.PortBindings ?? new Dictionary<string, IList<PortBinding>>();

            parameters.HostConfig.PortBindings.Add($"{containerPort}/tcp", new List<PortBinding>()
            {
                new PortBinding
                {
                    HostPort = hostPort.ToString(),
                },
            });

            parameters.ExposedPorts = parameters.ExposedPorts ?? new Dictionary<string, EmptyStruct>();
            parameters.ExposedPorts.Add($"{containerPort}/tcp", default(EmptyStruct));
        }

        public static void AddBind(this CreateContainerParameters parameters, string hostPath, string containerPath)
        {
            string bind = $"{hostPath}:{containerPath}";

            parameters.HostConfig = parameters.HostConfig ?? new HostConfig();
            parameters.HostConfig.Binds = parameters.HostConfig.Binds ?? new List<string>();
            parameters.HostConfig.Binds.Add(bind);
        }
    }
}
