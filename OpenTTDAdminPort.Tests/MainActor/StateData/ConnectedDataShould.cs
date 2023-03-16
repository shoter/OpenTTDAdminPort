using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using Xunit;
using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.MainActor.StateData
{
    public class ConnectedDataShould : BaseTestKit
    {
        public ConnectedDataShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void RemovePlayer_AndDoNotModifyPlayerListInInput()
        {
            var player = fix.Create<Player>();
            ConnectedData data = CreateData() with
            {
                Players = new Dictionary<uint, Player>() { { 1, player } },
            };

            var newData = data.DeletePlayer(1);

            Assert.Empty(newData.Players);
            Assert.Single(data.Players);
            Assert.Equal(player, data.Players.Values.First());
        }

        private ConnectedData CreateData()
        {
            var connectingData = new ConnectingData(
                probe,
                probe,
                fix.Create<ServerInfo>(),
                "client");

            connectingData.AdminServerInfo = fix.Create<AdminServerInfo>();
            return new ConnectedData(connectingData, probe);
        }
    }
}
