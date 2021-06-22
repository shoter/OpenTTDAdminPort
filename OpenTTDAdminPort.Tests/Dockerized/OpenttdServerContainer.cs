﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace OpenTTDAdminPort.Tests.Dockerized
{
    public class OpenttdServerContainer : ContainerApplication
    {
        public ServerInfo ServerInfo => new ServerInfo("127.0.0.1", Port, "admin_pass");

        public OpenttdServerContainer(DockerClient client) : base(client)
        {
        }

        protected override CreateContainerParameters OverrideContainerParameters(CreateContainerParameters options, int assignedPort)
        {
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), nameof(Dockerized), "openttd.cfg");
            options.AddPortBinding(Port, 3982);
            // For debugging purposes
            options.AddPortBinding(GetFreeTcpPort(), 3979);
            options.AddBind(configPath, "/config/openttd.cfg:ro");
            return base.OverrideContainerParameters(options, assignedPort);
        }

        protected override string ImageName => "redditopenttd/openttd";

        protected override string TagName => "1.11.2";

        protected override async Task WaitForContainerStart()
        {
            var logFactory = new LoggerFactory();
            logFactory.AddProvider(new DebugLoggerProvider());

            AdminPortClient client = null;
            while (client?.ConnectionState != AdminConnectionState.Connected)
            {
                try
                {
                    client = new AdminPortClient(ServerInfo, logFactory.CreateLogger<AdminPortClient>());
                    await client.Connect();
                }
                catch (Exception)
                {
                    if (client.ConnectionState != AdminConnectionState.ErroredOut)
                    {
                        await client.Disconnect();
                    }
                }
            }
            await client?.Disconnect();
        }
    }
}
