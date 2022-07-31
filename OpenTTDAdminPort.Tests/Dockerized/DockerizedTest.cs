using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            : base(testOutput)
        {
            this.serviceProvider = CreateServiceProvider(testOutput);
            this.application = serviceProvider.GetService<AlarmsDatabase>();

            dbOptions.Value.Returns(_ =>
            {
                return new DatabaseOptions()
                {
                    Name = "AlarmDb",
                    ConnectionString = application.ConnectionString,
                };
            });
        }

        private IServiceProvider CreateServiceProvider(ITestOutputHelper testOutput)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IDockerService, DockerService>();
            services.AddSingleton<IDockerImageService, DockerImageService>();
            services.AddSingleton<IDockerContainerService, DockerContainerService>();
            services.AddSingleton<IDockerProgressFactory, DockerProgressFactory>();
            services.AddSingleton<IDockerClient>(DockerClientProvider.Instance);
            services.AddSingleton<IAlarmRepository, AlarmRepository>();
            services.AddSingleton<ILastReceiveTimeRepository, LastReceiveTimeRepository>();
            services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
            services.AddTransient<AlarmsDatabase>();
            services.AddSingleton(dbOptions);

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
