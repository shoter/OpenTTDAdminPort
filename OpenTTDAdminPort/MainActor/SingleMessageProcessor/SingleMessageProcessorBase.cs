using System;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor.SingleMessageProcessor
{
    public abstract class SingleMessageProcessorBase<TAdminMessage> : ISingleMessageProcessor
    where TAdminMessage : IAdminMessage
    {
        internal abstract ConnectedData ProcessAdminMessage(
            ConnectedData data,
            TAdminMessage message);

        public ConnectedData ProcessAdminMessage(
            ConnectedData data,
            IAdminMessage message)
        {
            if (message is TAdminMessage tMessage)
            {
                return ProcessAdminMessage(
                    data,
                    tMessage);
            }

            throw new ArgumentException(nameof(message));
        }
    }
}