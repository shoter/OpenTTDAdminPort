using System;

using Docker.DotNet;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort.Tests.Dockerized.Containers;
using OpenTTDAdminPort.Tests.Dockerized.Images;
using OpenTTDAdminPort.Tests.Dockerized.Progress;
using OpenTTDAdminPort.Tests.Logging;

using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class DockerizedTest<TApp> : IDisposable
        where TApp : ContainerApplication
    {
        private bool disposedValue;

        protected readonly TApp application;
        protected readonly IServiceProvider serviceProvider;

        public DockerizedTest(ITestOutputHelper testOutput)
        {
            this.serviceProvider = CreateServiceProvider(testOutput);
            this.application = serviceProvider.GetService<TApp>();
        }

        private IServiceProvider CreateServiceProvider(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IDockerService, DockerService>();
            services.AddSingleton<IDockerImageService, DockerImageService>();
            services.AddSingleton<IDockerContainerService, DockerContainerService>();
            services.AddSingleton<IDockerProgressFactory, DockerProgressFactory>();
            services.AddSingleton<IDockerClient>(DockerClientProvider.Instance);

            services.AddLogging(builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(testOutput));
            });

            SetupServiceProvider(services);

            return services.BuildServiceProvider();
        }

        protected virtual void SetupServiceProvider(IServiceCollection services)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    application.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
