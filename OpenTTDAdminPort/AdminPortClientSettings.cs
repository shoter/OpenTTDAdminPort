using System;
using OpenTTDAdminPort.Networking.Watchdog;

namespace OpenTTDAdminPort
{
    public class AdminPortClientSettings
    {
        /// <summary>
        /// Specify interval between subsequent pings to the server used in <see cref="ConnectionWatchdog"/>
        /// Specify how long will be time between pings to the server.
        /// </summary>
        public TimeSpan WatchdogInterval { get; set; }

        public static AdminPortClientSettings Default = new AdminPortClientSettings()
        {
            WatchdogInterval = TimeSpan.FromSeconds(15),
        };
    }
}
