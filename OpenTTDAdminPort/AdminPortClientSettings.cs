using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort
{
    public class AdminPortClientSettings
    {
        public TimeSpan WatchdogInterval { get; set; }


        public static AdminPortClientSettings Default = new AdminPortClientSettings()
        {
            WatchdogInterval = TimeSpan.FromSeconds(15),
        };
    }
}
