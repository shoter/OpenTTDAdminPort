using Moq;
using OpenTTDAdminPort.States;
using OpenTTDAdminPort.Tests.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests
{
    public class AdminPortClientShould
    {
        AdminPortClientContext context;
        Mock<AdminPortTcpClientFake> tcpClientMock = new Mock<AdminPortTcpClientFake>();

        IAdminPortClient client;
        public AdminPortClientShould()
        {
            tcpClientMock.CallBase = true;

            client = new AdminPortClient(tcpClientMock.Object, null,
                new ServerInfo("127.0.0.1", 123, "LubiePlacki"));


        }
        
    }
}
