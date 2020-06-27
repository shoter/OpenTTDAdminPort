using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class AdminPortClient
    {
        private AdminPortClientContext Context { get; set; }

        private Dictionary<AdminConnectionState, IAdminPortClientState> StateRunners { get; } = new Dictionary<AdminConnectionState, IAdminPortClientState>();

        public event EventHandler<IAdminEvent>? EventReceived;

        public AdminPortClient(ServerInfo serverInfo)
        {
            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService), new AdminPortTcpClientReceiver(packetService), new MyTcpClient());
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo);
            Init(tcpClient, serverInfo);
        }

        internal AdminPortClient(IAdminPortTcpClient adminPortTcpClient, ServerInfo serverInfo)
        {
            Context = new AdminPortClientContext(adminPortTcpClient, "AdminPort", "1.0.0", serverInfo);
            Init(adminPortTcpClient, serverInfo);
        }

        private void Init(IAdminPortTcpClient adminPortTcpClient, ServerInfo serverInfo)
        {
            adminPortTcpClient.MessageReceived += AdminPortTcpClient_MessageReceived;
            adminPortTcpClient.Errored += AdminPortTcpClient_Errored;
        }

        private void AdminPortTcpClient_Errored(object sender, Exception e)
        {
            Context.state = AdminConnectionState.Errored;
        }

        private void AdminPortTcpClient_MessageReceived(object sender, IAdminMessage e)
        {
            StateRunners[Context.State].OnMessageReceived(e, Context);
        }

        private void Context_StateChanged(object sender, AdminConnectionStateChangedArgs e)
        {
            StateRunners[e.Old].OnStateEnd(Context);
            StateRunners[e.New].OnStateStart(Context);
        }

        public Task Connect() => StateRunners[Context.State].Connect(Context);

        public Task Disconnect() => StateRunners[Context.State].Disconnect(Context);

        void SendMessage(IAdminMessage message) => StateRunners[Context.State].OnMessageReceived(message, Context);
    }
}
