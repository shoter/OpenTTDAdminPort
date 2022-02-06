using Akka.Actor;

using System;

namespace OpenTTDAdminPort.Networking
{
    internal partial class AdminPortTcpClient : ReceiveActor 
    {
        internal void IdleReady()
        {
            ReceiveAsync<AdminPortTcpClientConnect>(async c =>
            {
                await tcpClient.ConnectAsync(c.Ip, c.Port);
                this.stream = tcpClient.GetStream();
                this.receiver = actorFactory.CreateActor(Context, sp => Props.Create(() => new AdminPortTcpClientReceiver(sp, stream)));
                Become(ConnectedReady);
            });
        }
        
    }
}
