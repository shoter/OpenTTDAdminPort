﻿using Moq;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Networking
{
    public class AdminPortConnectingStateShould : BaseStateShould
    {
        AdminPortConnectingState state;

        public AdminPortConnectingStateShould()
        {
            state = new AdminPortConnectingState(adminPacketServiceMock.Object);
        }

        [Fact]
        public void FillAdminUpdateSettings_OnProtocolMessage()
        {
            Dictionary<AdminUpdateType, UpdateFrequency> upFreqDic = new Dictionary<AdminUpdateType, UpdateFrequency>();
            upFreqDic[AdminUpdateType.ADMIN_UPDATE_CHAT] = UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC;
            var protocol = new AdminServerProtocolMessage(1, upFreqDic);

            state.OnMessageReceived(protocol, context);


            Assert.Equal(UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC, context.AdminUpdateSettings[AdminUpdateType.ADMIN_UPDATE_CHAT].UpdateFrequency);
            Assert.True(context.AdminUpdateSettings[AdminUpdateType.ADMIN_UPDATE_CHAT].Enabled);
        }

        [Fact]
        public void ShouldNotFillUpdateSettings_ThatWereNotSpecifiedInProtocol()
        {
            Dictionary<AdminUpdateType, UpdateFrequency> upFreqDic = new Dictionary<AdminUpdateType, UpdateFrequency>();
            upFreqDic[AdminUpdateType.ADMIN_UPDATE_CHAT] = UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC;
            var protocol = new AdminServerProtocolMessage(1, upFreqDic);

            state.OnMessageReceived(protocol, context);

            foreach (var us in context.AdminUpdateSettings)
            {
                if (us.Key == AdminUpdateType.ADMIN_UPDATE_CHAT)
                    continue;
                Assert.False(us.Value.Enabled);
            }
        }


        [Fact]
        public void CatchCorrectAdminServerInfo_WhenServerSendsWelcomePacket()
        {
            var welcome = new AdminServerWelcomeMessage()
            {
                IsDedicated = true,
                MapName = "SomeMapName",
                NetworkRevision = "1.2.3.4",
                ServerName = "SuperServer",
                CurrentDate = new OttdDate(10, 1, 15),
                Landscape = Landscape.LT_ARCTIC,
                MapHeight = 2,
                MapSeed = 33,
                MapWidth = 4
            };

            state.OnMessageReceived(welcome, context);

            Assert.True(context.AdminServerInfo.IsDedicated);
            Assert.Equal("SomeMapName", context.AdminServerInfo.MapName);
            Assert.Equal("1.2.3.4", context.AdminServerInfo.RevisionName);
            Assert.Equal("SuperServer", context.AdminServerInfo.ServerName);
        }

        [Fact]
        public void SendProperUpdateMessages_AfterWelcomeMessage()
        {
            AdminServerWelcomeMessage welcome = CreateWelcomeMessage();

            state.OnMessageReceived(welcome, context);

            Dictionary<AdminUpdateType, UpdateFrequency> FrequencyMessagesToSend = new Dictionary<AdminUpdateType, UpdateFrequency>()
            {
                { AdminUpdateType.ADMIN_UPDATE_CHAT, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC },
                { AdminUpdateType.ADMIN_UPDATE_CONSOLE, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC },
                { AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC }
            };

            foreach (var kp in FrequencyMessagesToSend)
            {
                tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg =>
                    msg is AdminUpdateFrequencyMessage &&
                    ((AdminUpdateFrequencyMessage)msg).UpdateType == kp.Key &&
                    ((AdminUpdateFrequencyMessage)msg).UpdateFrequency == kp.Value
                )), Times.Once);
            }

            tcpClientMock.Verify(x => x.SendMessage(It.Is<IAdminMessage>(msg =>
                msg is AdminPollMessage &&
                ((AdminPollMessage)msg).Argument == uint.MaxValue
                )), Times.Once);
        }

        [Fact]
        public void ChangeStateToConnected_AfterReceivingWelcomeMessage()
        {
            AdminServerWelcomeMessage welcome = CreateWelcomeMessage();

            state.OnMessageReceived(welcome, context);

            Assert.Equal(AdminConnectionState.Connected, context.State);
        }

        private static AdminServerWelcomeMessage CreateWelcomeMessage()
        {
            return new AdminServerWelcomeMessage()
            {
                IsDedicated = true,
                MapName = "SomeMapName",
                NetworkRevision = "1.2.3.4",
                ServerName = "SuperServer",
                CurrentDate = new OttdDate(10, 1, 15),
                Landscape = Landscape.LT_ARCTIC,
                MapHeight = 2,
                MapSeed = 33,
                MapWidth = 4
            };
        }
    }
}
