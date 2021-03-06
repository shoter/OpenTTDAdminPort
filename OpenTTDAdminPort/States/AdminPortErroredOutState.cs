﻿using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortErroredOutState : IAdminPortClientState
    {
        public Task Connect(IAdminPortClientContext context)
        {
            throw new AdminPortException("AdminPortClient had fatal error - unable to continue");
        }

        public Task Disconnect(IAdminPortClientContext context)
        {
            throw new AdminPortException("AdminPortClient had fatal error - unable to continue");
        }

        public void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context)
        {
        }

        public void OnStateEnd(IAdminPortClientContext context)
        {
        }

        public void OnStateStart(IAdminPortClientContext context)
        {
            context.MessagesToSend.Clear();
        }

        public void SendMessage(IAdminMessage message, IAdminPortClientContext context)
        {
            throw new AdminPortException("AdminPortClient had fatal error - unable to continue");
        }
    }
}
