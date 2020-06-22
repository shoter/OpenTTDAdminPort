using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Packets.MessageTransformers;
using OpenTTDAdminPort.Packets.PacketTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Packets
{
    internal class AdminPacketServiceFactory
    {
        internal IAdminPacketService Create()
        {
            IEnumerable<Type> packetTransformerTypes = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), GetType().Namespace)
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(ImplementsTypeMatcher.Create<IPacketTransformer>())
                .Find();

            IEnumerable<Type> messageTransformerTypes = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), GetType().Namespace)
            .WithTypeMatcher(new ClassTypeMatcher())
            .WithTypeMatcher(ImplementsTypeMatcher.Create<IMessageTransformer>())
            .Find();

            IPacketTransformer[] packetTransformers = new IPacketTransformer[packetTransformerTypes.Count()];
            IMessageTransformer[] messageTransformers = new IMessageTransformer[messageTransformerTypes.Count()];

            for(int i = 0;i < packetTransformers.Length; ++i)
            {
                packetTransformers[i] = (IPacketTransformer)Activator.CreateInstance(packetTransformerTypes.ElementAt(i));
            }

            for (int i = 0; i < messageTransformers.Length; ++i)
            {
                messageTransformers[i] = (IMessageTransformer)Activator.CreateInstance(messageTransformerTypes.ElementAt(i));
            }

            return new AdminPacketService(packetTransformers, messageTransformers);
        }

    }
}
