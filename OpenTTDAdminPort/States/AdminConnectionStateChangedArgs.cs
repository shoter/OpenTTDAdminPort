using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    public class AdminConnectionStateChangedArgs : EventArgs
    {
        public AdminConnectionState Old { get; }
        public AdminConnectionState New { get; }
        public AdminConnectionStateChangedArgs(AdminConnectionState old, AdminConnectionState newState)
        {
            Old = old;
            New = newState;
        }
    }
}
