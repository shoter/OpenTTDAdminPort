using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Events.Creators;
using OpenTTDAdminPort.Extensions;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events
{
    internal class AdminEventFactory : IAdminEventFactory
    {
        private readonly Dictionary<AdminMessageType, IEventCreator> creators = new Dictionary<AdminMessageType, IEventCreator>();
        private readonly ILogger logger;

        public AdminEventFactory(params IEventCreator[] eventCreators)
        {
            creators = eventCreators.ToDictionary(x => x.SupportedMessageType);
            logger = NullLogger<AdminEventFactory>.Instance;
        }

        public AdminEventFactory(ILogger<AdminEventFactory> logger)
        {
            this.logger = logger;

            Assembly assembly = typeof(AdminEventFactory).Assembly;

            var creatorTypes = new AssemblyTypeFinder(assembly, GetType().Namespace + ".Creators")
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

        public IAdminEvent? Create(in IAdminMessage adminMessage, in ConnectedData prev, in ConnectedData context)
        {
            IEventCreator? creator = null;
            try
            {
                if (!creators.ContainsKey(adminMessage.MessageType))
                {
                    return null;
                }

                creator = creators[adminMessage.MessageType];
                return creator.Create(adminMessage, prev, context);
            }
            catch
            {
                logger.LogError($"Error encountered while transforming {adminMessage}. Creator used - {creator}");
                logger.LogErrorJson(adminMessage, prev, context);
                throw;
            }
        }
    }
}
