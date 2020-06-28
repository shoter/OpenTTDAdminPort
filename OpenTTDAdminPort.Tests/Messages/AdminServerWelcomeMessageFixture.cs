using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Messages
{
    internal class AdminServerWelcomeMessageFixture
    {
        public AdminServerWelcomeMessage Build()
        {
            return new AdminServerWelcomeMessage()
            {
                IsDedicated = true,
                MapName = "SomeMapName",
                NetworkRevision = "1.2.3.4",
                ServerName = "SuperServer",
                CurrentDate = new OttdDate(10, 1, 15),
                Landscape = Landscape.LT_ARCTIC,
                MapHeight = 2,
                MapSeed = 33,
                MapWidth = 4
            };
        }
        
    }
}
