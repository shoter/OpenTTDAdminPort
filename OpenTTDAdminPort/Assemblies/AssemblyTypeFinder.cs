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

        private List<IAssemblyTypeMatcher> typeFilters = new List<IAssemblyTypeMatcher>();

        public AssemblyTypeFinder(Assembly assembly, string namespaceName)
        {
            this.namespaceName = namespaceName;
            this.assembly = assembly;
        }

        public AssemblyTypeFinder WithTypeFilter(IAssemblyTypeMatcher typeFilter)
        {
            this.typeFilters.Add(typeFilter);
            return this;
        }

        public IEnumerable<Type> Find()
        {
            IEnumerable<Type> types = assembly.GetTypes()
              .Where(t => String.Equals(t.Namespace, namespaceName, StringComparison.Ordinal));

            List<Type> ret = new List<Type>();

            foreach(var type in types)
            {
                bool canAdd = true;
                foreach(var f in typeFilters)
                {
                    if(!f.IsMatching(type))
                    {
                        canAdd = false;
                        break;
                    }
                }

                if (canAdd)
                    ret.Add(type);
            }

            return ret;
        }
    }
}
