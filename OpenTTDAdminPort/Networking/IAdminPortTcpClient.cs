﻿using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Networking
{
    internal interface IAdminPortTcpClient
    {
        event EventHandler<IAdminMessage> MessageReceived;
        event EventHandler<Exception> Errored;
        void SendMessage(IAdminMessage message);
        Task Start(string ip, int port);
        Task Stop(ITcpClient tcpClient);
        Task Restart(ITcpClient tcpClient);


        WorkState State { get; }
    }
}
