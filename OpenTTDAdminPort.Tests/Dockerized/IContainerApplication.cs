using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public interface IContainerApplication : IDisposable
    {
        /// <summary>
        /// Contains ports on which application is running.
        /// </summary>
        /// <remarks>
        /// Value is garbage before `Start` is completed.
        /// </remarks>
        int Port { get; }

        Task Start(string containerName, int? port = null);

        Task Stop();
    }
}
