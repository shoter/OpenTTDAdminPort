using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Networking
{
    internal class AdminPortTcpClientFake : IAdminPortTcpClient
    {
        public virtual WorkState State { get; private set; } = WorkState.NotStarted;
        public virtual event EventHandler<IAdminMessage> MessageReceived;
        public virtual event EventHandler<Exception> Errored;


        public virtual Task Restart(ITcpClient tcpClient)
        {
            return Task.CompletedTask;
        }

        public virtual void SendMessage(IAdminMessage message) { }

        public virtual Task Start(string ip, int port)
        {
            State = WorkState.Working;
            return Task.CompletedTask;
        }

        public virtual Task Stop(ITcpClient client)
        {
            State = WorkState.Stopped;
            return Task.CompletedTask;
        }
    }
}
