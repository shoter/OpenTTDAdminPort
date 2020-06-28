﻿using Moq;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.States;
using OpenTTDAdminPort.Tests.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests
{
    public class AdminPortClientShould
    {
        Mock<AdminPortTcpClientFake> tcpClientMock = new Mock<AdminPortTcpClientFake>();
        Mock<IAdminMessageProcessor> messageProcessorMock = new Mock<IAdminMessageProcessor>();

        IAdminPortClient client;
        public AdminPortClientShould()
        {
            tcpClientMock.CallBase = true;

            client = new AdminPortClient(tcpClientMock.Object, messageProcessorMock.Object,
                new ServerInfo("127.0.0.1", 123, "LubiePlacki"));
        }

        [Fact]
        public void NotConnect_AfterObjectIsConstructed()
        {
            tcpClientMock.Verify(x => x.Start(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
        
    }
}
