﻿namespace System
{
    internal static class DateTimeExtensions
    {
        public static string ToTime(this DateTime t) => $"{t:HH:mm:ss.fff}";
    }
}
