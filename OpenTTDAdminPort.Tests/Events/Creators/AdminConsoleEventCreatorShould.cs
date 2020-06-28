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
    public class AdminConsoleEventCreatorShould
    {
        AdminConsoleEventCreator creator = new AdminConsoleEventCreator();

        [Fact]
        public void HaveCorrectSupportMessageType() =>
            Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE, creator.SupportedMessageType);

        [Fact]
        public void CreateCorrectEvent()
        {
            var msg = new AdminServerConsoleMessage("ObiWan", "I have high ground");
            var ev = (AdminConsoleEvent)creator.Create(msg, Mock.Of<IAdminPortClientContext>());

            Assert.Equal("ObiWan", msg.Origin);
            Assert.Equal("I have high ground", msg.Message);
        }
    }
}
