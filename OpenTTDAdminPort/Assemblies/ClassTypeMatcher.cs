using System;

namespace OpenTTDAdminPort.Assemblies
{
    internal class ClassAssemblyMatcher : IAssemblyTypeMatcher
    {
        public bool IsMatching(Type type) => type.IsClass;
    }
}
