using Microsoft.Extensions.Logging;

namespace OpenTTDAdminPort.Tests.Dockerized.Progress
{
    public class DockerProgressFactory : IDockerProgressFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public DockerProgressFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public DockerProgress Create()
        {
            return new DockerProgress(this.loggerFactory.CreateLogger<DockerProgress>());
        }
    }
}
