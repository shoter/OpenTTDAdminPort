using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClient : IAdminPortTcpClient, IDisposable
    {
        public event EventHandler<IAdminMessage>? MessageReceived;
        public event EventHandler<Exception>? Errored;

        private ITcpClient tcpClient;

        private IAdminPortTcpClientSender sender;
        private IAdminPortTcpClientReceiver receiver;
        private string? ip;
        private int port;

        private readonly ILogger? logger;


        public WorkState State { get; set; } = WorkState.NotStarted;
        public AdminPortTcpClient(IAdminPortTcpClientSender sender, IAdminPortTcpClientReceiver receiver, ITcpClient tcpClient, ILogger? logger = null)
        { 
            this.sender = sender;
            this.receiver = receiver;
            this.tcpClient = tcpClient;
            this.logger = logger;

            sender.ErrorOcurred += (e, arg) => OnError(e, arg);
            receiver.ErrorOcurred += (e, arg) => OnError(e, arg);
            receiver.MessageReceived += (e, arg) => MessageReceived?.Invoke(e, arg);
        }

        public void SendMessage(IAdminMessage message)
        {
            sender.SendMessage(message);
        }

        public async Task Start(string ip, int port)
        {
            logger?.LogTrace($"Starting {nameof(AdminPortTcpClient)} on {ip}:{port}");
            if (State != WorkState.NotStarted && State != WorkState.Stopped)
            {
                State = WorkState.Errored;
                await Task.WhenAll(receiver.Stop(), sender.Stop());
                throw new AdminPortException("This Client is working atm. Please Stop it before starting it again.");
            }
            this.ip = ip;
            this.port = port;

            await tcpClient.ConnectAsync(ip, port);
            await Task.WhenAll(
                sender.Start(tcpClient.GetStream()),
                receiver.Start(tcpClient.GetStream()));

            State = WorkState.Working;
        }

        public async Task Stop(ITcpClient tcpClient)
        {
            if (State == WorkState.Working)
            {
                logger?.LogTrace($"Stopping {nameof(AdminPortTcpClient)} on {ip}:{port}");

                await Task.WhenAll(receiver.Stop(), sender.Stop());
                logger?.LogTrace($"Received and sender stopped for {ip}:{port}");
                this.tcpClient.Close();
                this.tcpClient.Dispose();
                this.tcpClient = tcpClient;
                this.State = WorkState.Stopped;
            }
        }

        public void OnError(object origin, Exception e)
        {
            logger?.LogTrace($"{origin} errored. Stopping tcp client for {ip}:{port}");
            State = WorkState.Errored;
            receiver?.Stop().Wait();
            sender?.Stop().Wait();
            Errored?.Invoke(this, e);
        }

        public async Task Restart(ITcpClient tcpClient)
        {
            Debug.Assert(this.ip != null);
            await Stop(tcpClient);
            await Start(this.ip, this.port);
        }

        public void Dispose()
        {
            tcpClient.Close();
        }
    }
}
