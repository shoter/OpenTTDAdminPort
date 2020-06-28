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
    public class AdminRconEventCreatorShould
    {
        [Fact]
        public void HaveCorrectSupportedType() =>
            Assert.Equal(AdminMessageType.ADMIN_PACKET_SERVER_RCON, new AdminRconEventCreator().SupportedMessageType);

        [Fact]
        public void CreateCorrectEvent()
        {
            var ev = (AdminRconEvent)new AdminRconEventCreator()
                .Create(new AdminRconMessage("nuke"), Mock.Of<IAdminPortClientContext>());

            Assert.Equal("nuke", ev.Message);
        }
        
    }
}
