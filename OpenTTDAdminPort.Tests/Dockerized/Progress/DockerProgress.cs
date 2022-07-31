using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace OpenTTDAdminPort.Tests.Dockerized.Progress
{
    public class DockerProgress : IProgress<JSONMessage>
    {
        private readonly ILogger logger;

        public DockerProgress(ILogger<DockerProgress> logger)
        {
            this.logger = logger;
        }

        public void Report(JSONMessage value)
        {
            this.logger.LogInformation($"{value.ProgressMessage}");
        }
    }
}
