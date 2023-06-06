using System;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerClientInfoMessageProcessor : SingleMessageProcessorBase<AdminServerClientInfoMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerClientInfoMessage message)
        {
            var player = new Player(
                message.ClientId,
                message.ClientName,
                DateTimeOffset.Now,
                message.Hostname,
                message.PlayingAs);
            return data.UpsertPlayer(player);
        }
    }
}