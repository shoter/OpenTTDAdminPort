using System;
using System.IO;
using System.Threading.Tasks;

using Docker.DotNet;
using Docker.DotNet.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

using OpenTTDAdminPort.Logging;
using OpenTTDAdminPort.Tests.Dockerized.Containers;

namespace OpenTTDAdminPort.Tests.Dockerized.Applications
{
    public class OpenttdServerContainer : ContainerApplication
    {
        public ServerInfo ServerInfo => new ServerInfo("127.0.0.1", Port, "admin_pass");

        protected override string ImageName => "redditopenttd/openttd";

        protected override string TagName => "1.11.2";

        internal Action<ILoggingBuilder> AdditionalBuilder { get; set; } = (_) => { };

        public OpenttdServerContainer(IDockerService dockerService, ILogger<MockServerContainer> logger)
         : base(dockerService, logger)
        {
        }

        protected override CreateContainerParameters OverrideContainerParameters(CreateContainerParametersExt options)
        {
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), nameof(Dockerized), "openttd.cfg");
            options.AddPortBinding(Port, 3982);
            options.AddPortBinding(GetFreeTcpPort(), 3979);
            options.AddBind(configPath, "/config/openttd.cfg:ro");

            return options;
        }

        protected override async Task<bool> CheckIfContainerIsRunning()
        {
            var client = new AdminPortClient(AdminPortClientSettings.Default, ServerInfo,
                logs =>
                {
                    logs.AddDebug();
                    AdditionalBuilder(logs);
                });

            logger.LogInformation($"Just before connect - ${client}");
            await client.Connect();

            return true;
        }
    }
}
