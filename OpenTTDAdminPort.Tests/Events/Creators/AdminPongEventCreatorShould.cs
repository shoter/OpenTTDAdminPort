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
    public class AdminPongEventCreatorShould
    {
        AdminPongEventCreator creator = new AdminPongEventCreator();

        [Fact]
        public void HaveCorrectSupportedMessageType() =>
            Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_PONG, creator.SupportedMessageType);

        [Fact]
        public void CreateCorrectEvent()
        {
            var ev = (AdminPongEvent) creator.Create(new AdminServerPongMessage(123u), Mock.Of<IAdminPortClientContext>());

            Assert.Equal(123u, ev.PongValue);
        }
        
    }
}
