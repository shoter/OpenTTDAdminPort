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
        private AdminPortClientContext Context { get; }

        private Dictionary<AdminConnectionState, IAdminPortClientState> StateRunners { get; } = new Dictionary<AdminConnectionState, IAdminPortClientState>();

        public event EventHandler<IAdminEvent> EventReceived;

        public AdminPortClient(ServerInfo serverInfo)
        {
            IAdminPacketService packetService = new AdminPacketServiceFactory().Create();
            IAdminPortTcpClient tcpClient = new AdminPortTcpClient(new AdminPortTcpClientSender(packetService), new AdminPortTcpClientReceiver(packetService), new MyTcpClient());
            Context = new AdminPortClientContext(tcpClient, "AdminPort", "1.0.0", serverInfo);
            Init();
        }

        internal AdminPortClient(IAdminPortTcpClient adminPortTcpClient, ServerInfo serverInfo)
        {
            Context = new AdminPortClientContext(adminPortTcpClient, "AdminPort", "1.0.0", serverInfo);
            Init();
            //Context.Errored += Context_Errored;

            //Context.StateChanged += Context_StateChanged;
            //Context.EventReceived += (_, e) => EventReceived(this, e);
        }

        private void Init()
        {
            Context.MessageReceived += Context_MessageReceived;


        }

        private void Context_Errored(object sender, Exception e)
        {
            throw new NotImplementedException();
        }

        private void Context_MessageReceived(object sender, IAdminMessage e)
        {
            throw new NotImplementedException();
        }

        private void Context_StateChanged(object sender, AdminConnectionStateChangedArgs e)
        {
            StateRunners[e.Old].OnStateEnd(Context);
            StateRunners[e.New].OnStateStart(Context);
        }

        public Task Connect() => StateRunners[Context.State].Connect(Context);

        public Task Disconnect() => StateRunners[Context.State].Disconnect(Context);

        void SendMessage(IAdminMessage message)
        {
            if (Context.State != AdminConnectionState.Connected)
            {
                Context.MessagesToSend.Enqueue(message);
            }
            else
            {
                if (Context.TcpClient == null)
                    throw new NullReferenceException(nameof(Context.TcpClient));

                while (Context.MessagesToSend.TryDequeue(out IAdminMessage msg))
                    Context.TcpClient.SendMessage(msg);
                Context.TcpClient.SendMessage(message);
            }
        }
    }
}
