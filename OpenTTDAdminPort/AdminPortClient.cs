using OpenTTDAdminPort.Common;
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

        private readonly IAdminMessageProcessor messageProcessor;

        private Dictionary<AdminConnectionState, IAdminPortClientState> StateRunners { get; } = new Dictionary<AdminConnectionState, IAdminPortClientState>();

        public event EventHandler<IAdminEvent>? EventReceived;

        public AdminPortClient(ServerInfo serverInfo)
        {
            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService), new AdminPortTcpClientReceiver(packetService), new MyTcpClient());
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo);
            messageProcessor = new AdminMessageProcessor();
            Init(tcpClient, serverInfo);
        }

        internal AdminPortClient(IAdminPortTcpClient adminPortTcpClient, IAdminMessageProcessor messageProcessor, ServerInfo serverInfo)
        {
            Context = new AdminPortClientContext(adminPortTcpClient, "AdminPort", "1.0.0", serverInfo);
            this.messageProcessor = messageProcessor;
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
            IAdminEvent? adminEvent = messageProcessor.ProcessMessage(e, Context);
            if(adminEvent != null)
                EventReceived?.Invoke(this, adminEvent);
        }

        private void Context_StateChanged(object sender, AdminConnectionStateChangedArgs e)
        {
            try
            {
                StateRunners[e.Old].OnStateEnd(Context);
                StateRunners[e.New].OnStateStart(Context);
            }
            catch (Exception)
            {
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        public async Task Connect()
        {
            try
            {
                await StateRunners[Context.State].Connect(Context);

                if (!(await TaskHelper.WaitUntil(() => Context.State == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    throw new AdminPortException("Admin port could not connect to the server");
                }
            }
            catch (Exception)
            {
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await StateRunners[Context.State].Disconnect(Context);

                if (!(await TaskHelper.WaitUntil(() => Context.State == AdminConnectionState.Idle, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    throw new AdminPortException("Encountered internal error.");
                }
            }
            catch (Exception)
            {
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }

        void SendMessage(IAdminMessage message)
        {
            try
            {
                StateRunners[Context.State].OnMessageReceived(message, Context);
            }
            catch (Exception)
            {
                Context.State = AdminConnectionState.ErroredOut;
                throw;
            }
        }
    }
}
