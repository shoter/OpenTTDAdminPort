﻿using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal class AdminPortTcpClient : IAdminPortTcpClient
    {
        public event EventHandler<IAdminMessage>? MessageReceived;
        public event EventHandler<Exception>? Errored;

        private ITcpClient tcpClient;

        private readonly IAdminPortTcpClientSender sender;
        private readonly IAdminPortTcpClientReceiver receiver;

        private readonly string ip;
        private readonly int port;

        public WorkState State { get; set; } = WorkState.NotStarted;
        public AdminPortTcpClient(IAdminPortTcpClientSenderFactory senderFactory, IAdminPortTcpClientReceiverFactory receiverFactory, ITcpClient tcpClient, string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.tcpClient = tcpClient;
            sender = senderFactory.Create(tcpClient);
            receiver = receiverFactory.Create(tcpClient);

            sender.ErrorOcurred += (e, arg) => OnError(e, arg);
            receiver.ErrorOcurred += (e, arg) => OnError(e, arg);
            receiver.MessageReceived += (e, arg) => MessageReceived?.Invoke(e, arg);
        }

        public void SendMessage(IAdminMessage message)
        {
            sender.SendMessage(message);
        }

        public async Task Start()
        {
            if (State != WorkState.NotStarted)
            {
                State = WorkState.Errored;
                await Task.WhenAll(receiver.Stop(), sender.Stop());
                throw new AdminPortException("This Client had been started before! You cannot start client more than 1 time");
            }

            await tcpClient.ConnectAsync(ip, port);
            await Task.WhenAll(receiver.Start(), sender.Start());
            State = WorkState.Working;
        }

        public async Task Stop()
        {
            if (State == WorkState.Working)
            {
                await Task.WhenAll(receiver.Stop(), sender.Stop());
                State = WorkState.Stopped;
            }
        }

        public void OnError(object caller, Exception e)
        {
            State = WorkState.Errored;
            receiver.Stop().Wait();
            sender.Stop().Wait();
            Errored?.Invoke(this, e);
        }
    }
}