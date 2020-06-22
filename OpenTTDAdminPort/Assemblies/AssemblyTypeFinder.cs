using OpenTTDAdminPort.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Common.Assemblies
{
    internal class AssemblyTypeFinder
    {
        private string namespaceName;
        private Assembly assembly;

        private List<ITypeMatcher> typeMatchers = new List<ITypeMatcher>();

        public AssemblyTypeFinder(Assembly assembly, string namespaceName)
        {
            this.namespaceName = namespaceName;
            this.assembly = assembly;
        }

        public AssemblyTypeFinder WithTypeMatcher(ITypeMatcher typeFilter)
        {
            this.typeMatchers.Add(typeFilter);
            return this;
        }

        public IEnumerable<Type> Find()
        {
            IEnumerable<Type> types = assembly.GetTypes()
              .Where(t => String.Equals(t.Namespace, namespaceName, StringComparison.Ordinal));

            List<Type> ret = new List<Type>();

            foreach(var type in types)
            {
                if(AreAllTypeMatchersMatching(type, typeMatchers))
                    ret.Add(type);
            }

            return ret;
        }

        private static bool AreAllTypeMatchersMatching(Type type, IEnumerable<ITypeMatcher> matchers)
        {
            bool allMatching = true;

            foreach (var m in matchers)
                if (!m.IsMatching(type))
                    return false;

            return allMatching;

        }
    }
}
