using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerClientUpdateMessageProcessor : SingleMessageProcessorBase<AdminServerClientUpdateMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerClientUpdateMessage message)
        {
            var player = data.Players[message.ClientId];
            return data.UpsertPlayer(
                player with
                {
                    Name = message.ClientName,
                    PlayingAs = message.PlayingAs,
                });
        }
    }
}