using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort
{
    public class AdminUpdateSetting
    {
        public bool Enabled { get; }

        public AdminUpdateType UpdateType { get; }

        public UpdateFrequency UpdateFrequency { get; }

        public AdminUpdateSetting(bool enabled, AdminUpdateType updateType, UpdateFrequency updateFrequency)
        {
            this.Enabled = enabled;
            this.UpdateType = updateType;
            this.UpdateFrequency = updateFrequency;
        }

        public override string ToString() => $"{this.UpdateType} - {this.UpdateFrequency}";
    }
}
