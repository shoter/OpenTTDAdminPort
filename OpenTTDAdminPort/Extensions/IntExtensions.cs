namespace System
{
    internal static class intExtensions
    {
        public static TimeSpan Millis(this int i) => TimeSpan.FromMilliseconds(i);

        public static TimeSpan Seconds(this int i) => TimeSpan.FromSeconds(i);

    }
}
