using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class DockerizePullProgress : IProgress<JSONMessage>
    {
        private readonly string imageName;

        public DockerizePullProgress(string imageName)
        {
            this.imageName = imageName;
        }

        public void Report(JSONMessage value)
        {
            Trace.WriteLine($"{imageName} pull progress msg: {value.ProgressMessage}");
        }
    }
}
