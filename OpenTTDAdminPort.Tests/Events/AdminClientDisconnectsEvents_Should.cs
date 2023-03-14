using System.Collections.Generic;
using AutoFixture;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using Xunit;

namespace OpenTTDAdminPort.Tests.Events
{
    public class AdminClientDisconnectsEvents_Should
    {
        private readonly Fixture fix = new();

        [Fact]
        public void BeCreated_WhenClient_NormallyQuits()
        {
            var msg = new AdminServerClientQuitMessage(1);
            var player = fix.Create<Player>() with { ClientId = 1 };
            var data = fix.Create<ConnectedData>() with { Players = new Dictionary<uint, Player>() { { 1, player } } };

            AdminEventFactory factory = new();
            var ev = factory.Create(msg, data, data);

            Assert.True(ev is AdminClientDisconnectEvent);
            var de = ev as AdminClientDisconnectEvent;
            Assert.Equal(player, de.Player);
        }
    }
}
