using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    internal class AdminServerClientErrorMessageProcessor : SingleMessageProcessorBase<AdminServerClientErrorMessage>
    {
        internal override ConnectedData ProcessAdminMessage(
            ConnectedData data,
            AdminServerClientErrorMessage message)
        {
            // There is an error in some version of openttd (mayeb in all?) where after client disconnects and server successfully
            // sends quitMessage server is going to send error message.
            // This is errorneous behaviour and if client is already disconnected we are going to ignor `error` messages connected with him.
            if (data.Players.ContainsKey(message.ClientId))
            {
                return data.DeletePlayer(message.ClientId);
            }

            return data;
        }
    }
}