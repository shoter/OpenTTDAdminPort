using System;

namespace OpenTTDAdminPort.Assemblies
{
    internal interface IAssemblyTypeMatcher
    {
        bool IsMatching(Type type);
    }
}
