using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort.Events
{
    public class AdminClientUpdateEvent : IAdminEvent
    {
        public Player Player { get; }

        public AdminEventType EventType => AdminEventType.ClientUpdate;

        public AdminClientUpdateEvent(Player player)
        {
            this.Player = player;
        }
    }
}
