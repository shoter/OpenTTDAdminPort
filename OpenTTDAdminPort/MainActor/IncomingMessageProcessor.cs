using System;
using System.Collections.Concurrent;
using OpenTTDAdminPort.MainActor.SingleMessageProcessor;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private ConcurrentDictionary<Type, ISingleMessageProcessor<>>

        public ConnectedData ProcessAdminMessage(
            ConnectedData initial,
            IAdminMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}