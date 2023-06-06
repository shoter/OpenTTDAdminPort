using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Events.Creators;
using OpenTTDAdminPort.MainActor.SingleMessageProcessor;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.MainActor
{
    public class IncomingMessageProcessor : IIncomingMessageProcessor
    {
        private ConcurrentDictionary<Type, ISingleMessageProcessor> processors = new();

        public IncomingMessageProcessor()
        {
            Assembly assembly = typeof(ISingleMessageProcessor).Assembly;

            var processorTypes = new AssemblyTypeFinder(assembly, GetType().Namespace!)
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(new ImplementsTypeMatcher(typeof(ISingleMessageProcessor)))
                .Find();

            foreach(var processorType in processorTypes)
            {
                var instance = Activator.CreateInstance(processorType) as ISingleMessageProcessor;
                processors[processorType] = instance!;
            }
        }

        public ConnectedData ProcessAdminMessage(
            ConnectedData initial,
            IAdminMessage message) => processors[message.GetType()]
            .ProcessAdminMessage(
                initial,
                message);
    }
}