using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OpenTTDAdminPort.States
{
    internal class AdminPortConnectingState : BaseAdminPortClientState
    {
        private readonly Timer timer = new Timer(10_000);

        public override void OnMessageReceived(IAdminMessage message, IAdminPortClientContext context)
        {
            // DTODO: This state also requires some kind of watchdog. 10s to complete?
            switch (message.MessageType)
            {
                case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
                    {
                        var msg = (AdminServerProtocolMessage)message;

                        foreach (var s in msg.AdminUpdateSettings)
                        {
                            //this.logger.LogInformation($"Update settings {s.Key} - {s.Value}");
                            context.AdminUpdateSettings.TryUpdate(s.Key, new AdminUpdateSetting(true, s.Key, s.Value), context.AdminUpdateSettings[s.Key]);
                        }

                        break;
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
                    {
                        var msg = (AdminServerWelcomeMessage)message;
                        // TODO: It will be not needed when we will have init accessors - then it will be easy to do non-null props that are accessed only through object initializer.
                        // Hate this line of code ATM :<.
                        Debug.Assert(msg.MapName != null && msg.NetworkRevision != null && msg.ServerName != null);


                        context.AdminServerInfo = new AdminServerInfo()
                        {
                            IsDedicated = msg.IsDedicated,
                            MapName = msg.MapName,
                            RevisionName = msg.NetworkRevision,
                            ServerName = msg.ServerName
                        };

                        timer.Stop();
                        context.State = AdminConnectionState.Connected;

                        //this.logger.LogInformation($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} - connected");
                        break;
                    }
            }
        }

        public override void OnStateStart(IAdminPortClientContext context)
        {
            // stop timer if it was started in the past.
            timer.Stop();

            timer.Elapsed += (_, __) =>
            {
                if(context.State == AdminConnectionState.Connecting)
                {
                    context.State = AdminConnectionState.Errored;
                }
            };

            timer.Start();

            base.OnStateStart(context);
        }

        public override Task Connect(IAdminPortClientContext context)
        {
            return Task.CompletedTask;
        }

        public override Task Disconnect(IAdminPortClientContext context)
        {
            context.State = AdminConnectionState.Disconnecting;
            return Task.CompletedTask;
        }
    }
}
