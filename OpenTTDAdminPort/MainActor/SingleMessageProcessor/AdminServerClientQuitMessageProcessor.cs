using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerClientQuitMessageProcessor : SingleMessageProcessorBase<AdminServerClientQuitMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerClientQuitMessage message)
        {
            // This if here is just in case if there is a chance that we try to remove player that was somehow not registered by the server.
            // You can call it defensive programming or smth.
            if (data.Players.ContainsKey(message.ClientId))
            {
                return data.DeletePlayer(message.ClientId);
            }

            return data;
        }
    }
}