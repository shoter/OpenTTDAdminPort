using Moq;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Events.Creators;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Events.Creators
{
    public class AdminChatEventCreatorShould
    {
        AdminChatEventCreator creator = new AdminChatEventCreator();

        [Fact]
        public void SupportCorrectMessageType() =>
            Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CHAT, creator.SupportedMessageType);

        [Fact]
        public void CreateCorrectEvent()
        {
            ConcurrentDictionary<uint, Player> playerDic = new ConcurrentDictionary<uint, Player>();
            playerDic.TryAdd(11u, new Player(11u, "Johny", DateTimeOffset.Now, "127.0.0.1"));
            Mock<IAdminPortClientContext> contextMock = new Mock<IAdminPortClientContext>();
            contextMock.SetupGet(x => x.Players).Returns(playerDic);

            var msg = new AdminServerChatMessage()
            {
                ChatDestination = ChatDestination.DESTTYPE_BROADCAST,
                ClientId = 11u,
                Data = 1,
                Message = "Hello There",
                NetworkAction = NetworkAction.NETWORK_ACTION_CHAT
            };

            var ev = (AdminChatMessageEvent)creator.Create(msg, contextMock.Object);

            Assert.Equal(11u, ev.Player.ClientId);
            Assert.Equal("Johny", ev.Player.Name);
            Assert.Equal(ChatDestination.DESTTYPE_BROADCAST, msg.ChatDestination);
            Assert.Equal(NetworkAction.NETWORK_ACTION_CHAT, msg.NetworkAction);
        }
    }
}
