using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Dockerized.Containers
{
    public interface IDockerContainerService
    {
        Task RemoveContainer(string containerNameOrId, CancellationToken token = default);

        Task StopContainer(string containerNameOrId, CancellationToken token = default);

        public async Task StopAndRemoveContainer(string containerNameOrId, CancellationToken token = default)
        {
            await StopContainer(containerNameOrId, token);
            await RemoveContainer(containerNameOrId, token);
        }
    }
}
