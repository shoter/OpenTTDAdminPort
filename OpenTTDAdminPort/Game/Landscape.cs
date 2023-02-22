using static OpenTTDAdminPort.Game.Landscape;

namespace OpenTTDAdminPort.Game
{
    public enum Landscape
    {
        LT_TEMPERATE = 0,
        LT_ARCTIC = 1,
        LT_TROPIC = 2,
        LT_TOYLAND = 3,
    }

    public static class LandscapeExtensions
    {
        public static string ToHumanReadable(Landscape l)
        {
            return l switch
            {
                LT_TEMPERATE => "Temperate",
                LT_ARCTIC => "Arctic",
                LT_TROPIC => "Tropic",
                LT_TOYLAND => "Toyland",
                _ => "Unknown",
            };
        }
    }
}
