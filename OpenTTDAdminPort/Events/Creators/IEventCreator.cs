using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Events.Creators
{
    internal interface IEventCreator
    {
        AdminMessageType SupportedMessageType { get; }

        IAdminEvent? Create(in IAdminMessage message, in IAdminPortClientContext context);
    }
}
