using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    public class AdminPortClientContext
    {
        internal TcpClient? TcpClient { get; set; }

        internal event EventHandler<IAdminEvent>? EventReceived;
        internal event EventHandler<AdminConnectionState>? StateChanged;

        internal string ClientName { get; }

        internal string ClientVersion { get; }

        private AdminConnectionState state;
        internal AdminConnectionState State
        {
            get => state;
            set
            {
                state = value;
                StateChanged?.Invoke(this, state);
            }
        }
        internal ConcurrentQueue<IAdminMessage> MessagesToSent { get; } = new ConcurrentQueue<IAdminMessage>();

        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        public AdminPortClientContext(string clientName, string clientVersion)
        {
            this.ClientName = ClientName;
            this.ClientVersion = clientVersion;
            this.state = AdminConnectionState.Idle;
        }
    }
}
