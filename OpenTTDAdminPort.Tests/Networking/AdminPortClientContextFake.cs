using Moq;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Networking
{
    internal class AdminPortClientContextFake : IAdminPortClientContext
    {
        public AdminPortClientContextFake()
        {
            FakeTcpClient = new Mock<AdminPortTcpClientFake>();
            FakeTcpClient.CallBase = true;

        }
        public IAdminPortTcpClient TcpClient => FakeTcpClient.Object;

        public Mock<AdminPortTcpClientFake> FakeTcpClient;
        public string ClientName => throw new NotImplementedException();

        public string ClientVersion => throw new NotImplementedException();

        public ServerInfo ServerInfo { get; set; } =  new ServerInfo("127.0.0.1", 123, "LubiePlacki");

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings => throw new NotImplementedException();

        public ConcurrentDictionary<uint, Player> Players => throw new NotImplementedException();

        public AdminServerInfo AdminServerInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AdminConnectionState State { get; set; } = AdminConnectionState.Idle;

        public ConcurrentQueue<IAdminMessage> MessagesToSend => throw new NotImplementedException();

        public event EventHandler<IAdminMessage> MessageReceived;
        public event EventHandler<AdminConnectionStateChangedArgs> StateChanged;
    }
}
