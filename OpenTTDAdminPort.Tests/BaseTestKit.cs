using Akka.Configuration;
using Akka.TestKit;
using Akka.TestKit.Xunit2;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using OpenTTDAdminPort.Akkas;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.Tests.Logging;
using OpenTTDAdminPort.Tests.Networking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests
{
    public class BaseTestKit : TestKit
    {
        protected readonly IServiceProvider defaultServiceProvider;

        protected readonly Fixture fix = new();

        private readonly DummyTcpClient tcpClient = new DummyTcpClient();

        private readonly Mock<NetworkingActorFactory> networkingActorFactory;

        protected TestProbe probe;

        public BaseTestKit(ITestOutputHelper testOutputHelper)
          : base(
                ConfigurationFactory.ParseString("\r\n                akka.test.testkit.debug = true\r\n                akka.log-dead-letters-during-shutdown = true\r\n                akka.actor.debug.receive = true\r\n                akka.actor.debug.autoreceive = true\r\n                akka.actor.debug.lifecycle = true\r\n                akka.actor.debug.event-stream = true\r\n                akka.actor.debug.unhandled = true\r\n                akka.actor.debug.fsm = true\r\n                akka.actor.debug.router-misconfiguration = true\r\n                akka.log-dead-letters = true\r\n                akka.loglevel = DEBUG\r\n                akka.stdout-loglevel = DEBUG")
                , null, testOutputHelper)
        {

            defaultServiceProvider = new ServiceCollection()
                .AddSingleton<ITcpClient>(tcpClient)
                .AddSingleton<IAdminPacketService>(new AdminPacketServiceFactory().Create())
                .AddSingleton<IActorFactory, ActorFactory>()
                .AddSingleton<INetworkingActorFactory>(sp => networkingActorFactory.Object)
                .AddLogging(logging =>
                {
                    logging.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .BuildServiceProvider();

            networkingActorFactory = new(defaultServiceProvider);

            probe = CreateTestProbe();
        }

    }
}
