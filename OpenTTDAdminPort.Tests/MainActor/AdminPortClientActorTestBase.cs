using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.MainActor;
using OpenTTDAdminPort.Tests.Networking;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.MainActor
{
    internal class AdminPortClientActorTestBase : BaseTestKit
    {
        private readonly IActorRef mainActor;

        private readonly IActorRef tcpFake;

        public AdminPortClientActorTestBase(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }
    }
}
