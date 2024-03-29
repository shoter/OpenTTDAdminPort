﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort
{
    public interface IAdminPortClient
    {
        AdminConnectionState ConnectionState { get; }

        ServerInfo ServerInfo { get; }

        void SendMessage(IAdminMessage message);

        void SendGlobalChatMessage(string message);

        void SetAdminEventHandler(Action<IAdminEvent> action);

        Task Connect(ILogger? test = null);

        Task Disconnect();

        Task<ServerStatus> QueryServerStatus(CancellationToken token = default);

        Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, CancellationToken token = default)
            where TEvent : IAdminEvent;

        Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, Func<TEvent, bool> func, CancellationToken token = default)
            where TEvent : IAdminEvent;

        Task<TEvent> WaitForEvent<TEvent>(IAdminMessage messageToSend, TimeSpan timeout, CancellationToken token = default)
            where TEvent : IAdminEvent;

        Task<IAdminEvent> WaitForEvent(IAdminMessage messageToSend, Func<IAdminEvent, bool> func, TimeSpan timeout, CancellationToken token = default);

        /// <remarks>
        /// This method queries internal data about admin port client. Do not use unless you want to know about internals of admin port client.
        /// </remarks>
        Task<IMainData> GetMainData(CancellationToken token = default);
    }
}
