﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.MainActor
{
    public enum MainState
    {
        Idle,
        Connecting,
        Connected,
        Disconnected,
        Errored,
    }
}
