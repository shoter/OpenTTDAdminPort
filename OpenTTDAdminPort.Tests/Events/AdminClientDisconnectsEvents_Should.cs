using System.Collections.Generic;
using AutoFixture;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Events
{
    public class AdminClientDisconnectsEvents_Should : BaseTestKit
    {
        public AdminClientDisconnectsEvents_Should(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void BeCreated_WhenClient_NormallyQuits()
        {
            var msg = new AdminServerClientQuitMessage(1);
            var player = fix.Create<Player>() with { ClientId = 1 };
            var data = CreateConnectedData() with
            { Players = new Dictionary<uint, Player>() { { 1, player } } };

            AdminEventFactory factory = new(defaultServiceProvider.GetRequiredService<ILogger<AdminEventFactory>>());
            var ev = factory.Create(msg, data, data);

            Assert.True(ev is AdminClientDisconnectEvent);
            var de = ev as AdminClientDisconnectEvent;
            Assert.Equal(player, de.Player);
        }

        [Fact]
        public void BeCreated_WhenClient_ExceptionallyQuits()
        {
            var msg = new AdminServerClientErrorMessage(1, 0);
            var player = fix.Create<Player>() with { ClientId = 1 };
            var data = CreateConnectedData() with
            { Players = new Dictionary<uint, Player>() { { 1, player } } };

            AdminEventFactory factory = new(defaultServiceProvider.GetRequiredService<ILogger<AdminEventFactory>>());
            var ev = factory.Create(msg, data, data);

            Assert.True(ev is AdminClientDisconnectEvent);
            var de = ev as AdminClientDisconnectEvent;
            Assert.Equal(player, de.Player);
        }

        private ConnectedData CreateConnectedData()
        {
            var connectingData = new ConnectingData(
                                probe,
                                probe,
                                fix.Create<ServerInfo>(),
                                "clientName");
            connectingData.AdminServerInfo = fix.Create<AdminServerInfo>();
            return new ConnectedData(connectingData, probe);
        }
    }
}
