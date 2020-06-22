using System;

namespace OpenTTDAdminPort.Assemblies
{
    /// <summary>
    /// Checks whether given type is a class.
    /// </summary>
    /// <seealso cref="OpenTTDAdminPort.Assemblies.ITypeMatcher" />
    internal class ClassTypeMatcher : ITypeMatcher
    {
        public bool IsMatching(Type type) => type.IsClass;
    }
}
