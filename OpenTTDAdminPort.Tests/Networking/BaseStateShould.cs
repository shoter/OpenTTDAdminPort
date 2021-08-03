using Microsoft.Extensions.Logging.Abstractions;

using Moq;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class BaseStateShould
    {
        internal Mock<AdminPortTcpClientFake> tcpClientMock = new Mock<AdminPortTcpClientFake>();
        internal Mock<IAdminPacketService> adminPacketServiceMock = new Mock<IAdminPacketService>();
        internal IAdminPortClientContext context;


        public BaseStateShould()
        {
            tcpClientMock.CallBase = true;
            context = new AdminPortClientContext(tcpClientMock.Object, "adminPortTest", "1.0.0.0",
                    new ServerInfo("127.0.0.1", 123, "LubiePlacki"), NullLogger.Instance);
        }

    }
}
