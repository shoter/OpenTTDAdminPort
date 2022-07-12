using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Packets.MessageTransformers;
using OpenTTDAdminPort.Packets.PacketTransformers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTTDAdminPort.Packets
{
    internal class AdminPacketServiceFactory
    {
        internal IAdminPacketService Create()
        {
            IEnumerable<Type> packetTransformerTypes = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), $"{GetType().Namespace}.PacketTransformers")
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(ImplementsTypeMatcher.Create<IPacketTransformer>())
                .Find();

            IEnumerable<Type> messageTransformerTypes = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), $"{GetType().Namespace}.MessageTransformers")
            .WithTypeMatcher(new ClassTypeMatcher())
            .WithTypeMatcher(ImplementsTypeMatcher.Create<IMessageTransformer>())
            .Find();

            IPacketTransformer[] packetTransformers = new IPacketTransformer[packetTransformerTypes.Count()];
            IMessageTransformer[] messageTransformers = new IMessageTransformer[messageTransformerTypes.Count()];

            for (int i = 0; i < packetTransformers.Length; ++i)
            {
                Type type = packetTransformerTypes.ElementAt(i);
                object? packetTransformer = Activator.CreateInstance(type);

                if (packetTransformer == null)
                {
                    throw new AdminPortException($"Could not create {type.Name}");
                }

                packetTransformers[i] = (IPacketTransformer)packetTransformer;
            }

            for (int i = 0; i < messageTransformers.Length; ++i)
            {
                Type type = messageTransformerTypes.ElementAt(i);
                object? messageTransformer = Activator.CreateInstance(type);

                if (messageTransformer == null)
                {
                    throw new AdminPortException($"Could not create {type.Name}");
                }

                messageTransformers[i] = (IMessageTransformer)messageTransformer;
            }

            return new AdminPacketService(packetTransformers, messageTransformers);
        }

    }
}
