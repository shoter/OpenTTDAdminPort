using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortErroredOutStateShould : BaseStateShould
    {
        AdminPortErroredOutState state = new AdminPortErroredOutState();

        [Fact]
        public void ThrowException_ForApiUsedByUser()
        {
            Assert.Throws<AdminPortException>(() => state.SendMessage(Mock.Of<IAdminMessage>(), context));
            Assert.ThrowsAsync<AdminPortException>(async () => await state.Connect(context));
            Assert.ThrowsAsync<AdminPortException>(async () => await state.Disconnect(context));
        }
    }
}
