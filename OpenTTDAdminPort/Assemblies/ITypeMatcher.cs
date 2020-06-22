using System;

namespace OpenTTDAdminPort.Assemblies
{
    internal interface ITypeMatcher
    {
        bool IsMatching(Type type);
    }
}
