using Moq;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Events.Creators;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Events.Creators
{
    public class AdminEventFactoryShould
    {
        [Fact]
        public void ReturnNull_WhenItCannotHandleSpecificMessageType()
        {
            var factory = new AdminEventFactory();

            Assert.Null(factory.Create(Mock.Of<IAdminMessage>(), Mock.Of<IAdminPortClientContext>()));
        }

        [Fact]
        public void ConstructMessage()
        {
            var creatorMock = new Mock<IEventCreator>();
            var retEv = Mock.Of<IAdminEvent>();
            creatorMock.SetupGet(x => x.SupportedMessageType).Returns(AdminMessageType.ADMIN_PACKET_SERVER_PONG);
            creatorMock.Setup(x => x.Create(It.Ref<IAdminMessage>.IsAny, It.Ref<IAdminPortClientContext>.IsAny)).Returns(retEv);
            var factory = new AdminEventFactory(creatorMock.Object);

            Assert.Equal(retEv, factory.Create(new AdminServerPongMessage(123u), Mock.Of<IAdminPortClientContext>()));
        }

        [Fact]
        public void ProperlyLocateEventCreators()
        {
            var factory = new AdminEventFactory();

            var pongEv = (AdminPongEvent)factory.Create(new AdminServerPongMessage(11u), Mock.Of<IAdminPortClientContext>());
            Assert.Equal(11u, pongEv.PongValue);
        }

    }
}
