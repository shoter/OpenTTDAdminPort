﻿using Akka.Actor;

using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.Messages;
using OpenTTDAdminPort.Messages;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenTTDAdminPort.MainActor.StateData
{
    public class ConnectedData : IMainData
    {
        public IActorRef TcpClient { get; }

        public ServerInfo ServerInfo { get; }

        public string ClientName { get; }

        public IActorRef Watchdog { get; }

        public Dictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } 

        public AdminServerInfo AdminServerInfo { get; set; }

        public Dictionary<uint, Player> Players { get; } = new();

        public ConnectedData(ConnectingData data, IActorRef watchdog)
        {
            Debug.Assert(data.AdminServerInfo != null);

            this.TcpClient = data.TcpClient;
            this.ServerInfo = data.ServerInfo;
            this.ClientName = data.ClientName;
            this.AdminUpdateSettings = data.AdminUpdateSettings;
            this.Watchdog = watchdog;
            this.AdminServerInfo = data.AdminServerInfo!;
        }
    }
}
