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

            var processorTypes = new AssemblyTypeFinder(
                    assembly,
                    typeof(ISingleMessageProcessor).Namespace!)
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(new ImplementsTypeMatcher(typeof(ISingleMessageProcessor)))
                .WithTypeMatcher(new NotAbstractTypeMatcher())
                .Find()
                .ToList(); // it is easier to debug with ToList :D

            foreach (var processorType in processorTypes)
            {
                var messageType = processorType.BaseType!.GenericTypeArguments.First();
                var instance = Activator.CreateInstance(processorType) as ISingleMessageProcessor;
                processors[messageType] = instance!;
            }
        }

        public ConnectedData ProcessAdminMessage(
            ConnectedData initial,
            IAdminMessage message)
        {
            var messageType = message.GetType();
            if (!processors.ContainsKey(messageType))
            {
                return initial;
            }

            return processors[messageType]
                .ProcessAdminMessage(
                    initial,
                    message);
        }
    }
}