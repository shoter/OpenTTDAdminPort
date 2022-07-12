using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Events.Creators;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events
{
    internal class AdminEventFactory : IAdminEventFactory
    {
        private readonly Dictionary<AdminMessageType, IEventCreator> creators = new Dictionary<AdminMessageType, IEventCreator>();

        public AdminEventFactory(params IEventCreator[] eventCreators)
        {
            creators = eventCreators.ToDictionary(x => x.SupportedMessageType);
        }

        public AdminEventFactory()
        {
            var creatorTypes = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), GetType().Namespace + ".Creators")
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(new ImplementsTypeMatcher(typeof(IEventCreator)))
                .Find();

            var creators = new IEventCreator[creatorTypes.Count()];

            for (int i = 0; i < creators.Length; ++i)
            {
                creators[i] = (IEventCreator)Activator.CreateInstance(creatorTypes.ElementAt(i))!;
            }

            this.creators = creators.ToDictionary(x => x.SupportedMessageType);
        }

        public IAdminEvent? Create(in IAdminMessage adminMessage, in ConnectedData context)
        {
            if (!creators.ContainsKey(adminMessage.MessageType))
            {
                return null;
            }

            return creators[adminMessage.MessageType].Create(adminMessage, context);
        }
    }
}
