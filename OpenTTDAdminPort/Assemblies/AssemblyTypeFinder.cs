using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Assemblies;

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
              .Where(t => string.Equals(t.Namespace, namespaceName, StringComparison.Ordinal));

            return types.Where(type => typeMatchers.All(tm => tm.IsMatching(type)));
        }
    }
}
