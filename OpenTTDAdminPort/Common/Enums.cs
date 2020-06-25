using System;
using System.Linq;

namespace OpenTTDAdminPort.Common
{
    internal class Enums
    {
        internal static T[] ToArray<T>()
            where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }
}
