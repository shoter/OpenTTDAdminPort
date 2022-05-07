using Moq;

using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Tests.Messages;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace OpenTTDAdminPort.Tests
{
    public class AdminPortClientShould
    {
        //IAdminPortClient client;
        //public AdminPortClientShould()
        //{
        //    tcpClientMock.CallBase = true;

        //    client = new AdminPortClient(AdminPortClientSettings.Default,
        //        new ServerInfo("127.0.0.1", 123, "LubiePlacki"));
        //}

        //[Fact]
        //public void NotConnect_AfterObjectIsConstructed()
        //{
        //    tcpClientMock.Verify(x => x.Start(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        //}

        //[Fact]
        //public void StartConnectToServer_WhenConnectWasExecutedForFirstTime()
        //{
        //    client.Connect();
        //    tcpClientMock.Verify(x => x.Start(client.ServerInfo.ServerIp, client.ServerInfo.ServerPort));
        //    Assert.Equal(AdminConnectionState.Connecting, client.ConnectionState);
        //}

        //[Fact]
        //public async Task ErrorOut_AfterConnecting_WhenStateWillNotTurnIntoConnected_After10Seconds()
        //{
        //    await Assert.ThrowsAsync<AdminPortException>(async () => await client.Connect());
        //    Assert.Equal(AdminConnectionState.ErroredOut, client.ConnectionState);
        //}

        //[Fact]
        //public async Task ConnectToServer()
        //{
        //    Task connectTask = client.Connect();
        //    Assert.Equal(AdminConnectionState.Connecting, client.ConnectionState);
        //    // tcp client should receive connect message by now.
        //    tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg => msg is AdminJoinMessage)));
        //    // we can send welcome messages to our dear client.
        //    tcpClientMock.Raise(x => x.MessageReceived += null, this, new AdminServerProtocolMessage(0, new Dictionary<Game.AdminUpdateType, Game.UpdateFrequency>()));
        //    tcpClientMock.Raise(x => x.MessageReceived += null, this, new AdminServerWelcomeMessageFixture().Build());
        //    // client should be now connected.
        //    Assert.Equal(AdminConnectionState.Connected, client.ConnectionState);
        //    // it will throw exception if this will not connect. This is basicly assertion of this test.
        //    await connectTask;
        //}

        //[Fact]
        //public async Task TurnIntoConnecting_WhenTcpClientErrorsOut()
        //{
        //    await Connect();
        //    tcpClientMock.Raise(x => x.Errored += null, this, new Exception("Boom!"));
        //    Assert.Equal(AdminConnectionState.Connecting, client.ConnectionState);
        //}

        //[Fact]
        //public async Task BeAbleToSendMessage_AfterConnecting()
        //{
        //    await Connect();
        //    var msg = Mock.Of<IAdminMessage>();
        //    client.SendMessage(msg);
        //    tcpClientMock.Verify(x => x.SendMessage(msg), Times.Once);
        //}

        //private async Task Connect()
        //{
        //    Task connectTask = client.Connect();
        //    Assert.Equal(AdminConnectionState.Connecting, client.ConnectionState);
        //    // tcp client should receive connect message by now.
        //    tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg => msg is AdminJoinMessage)));
        //    // we can send welcome messages to our dear client.
        //    tcpClientMock.Raise(x => x.MessageReceived += null, this, new AdminServerProtocolMessage(0, new Dictionary<Game.AdminUpdateType, Game.UpdateFrequency>()));
        //    tcpClientMock.Raise(x => x.MessageReceived += null, this, new AdminServerWelcomeMessageFixture().Build());
        //    // client should be now connected.
        //    Assert.Equal(AdminConnectionState.Connected, client.ConnectionState);
        //    await connectTask;
        //}
    }
}
