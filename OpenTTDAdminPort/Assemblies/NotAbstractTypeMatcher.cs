using System;

namespace OpenTTDAdminPort.Assemblies
{
    public class NotAbstractTypeMatcher : ITypeMatcher
    {
        public bool IsMatching(Type type)
        {
            return !type.IsAbstract;
        }
    }
}