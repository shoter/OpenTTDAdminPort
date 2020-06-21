using System.ComponentModel;

namespace OpenTTDAdminPort.Common
{
    public enum Landscape
    {
        [Description("Temperate")]
        LT_TEMPERATE = 0,
        [Description("Arctic")]
        LT_ARCTIC = 1,
        [Description("Tropic")]
        LT_TROPIC = 2,
        [Description("Toyland")]
        LT_TOYLAND = 3,
    }
}
