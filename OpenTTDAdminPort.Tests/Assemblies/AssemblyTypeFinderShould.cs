using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Tests.Assemblies.TestTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Assemblies
{
    public class AssemblyTypeFinderShould
    {
        [Fact]
        public void FindProperClasses_BasedOnTypeMatchers()
        {
            var finder = new AssemblyTypeFinder(Assembly.GetExecutingAssembly(), GetType().Namespace + ".TestTypes")
                .WithTypeMatcher(new ClassTypeMatcher())
                .WithTypeMatcher(new ImplementsTypeMatcher(typeof(IAnimal)));

            // Sometimes there is no full info about given type. It's better to compare results by guid
            var foundTypes = finder.Find().Select(x => x.GUID);

            Assert.Contains(typeof(Cat).GUID, foundTypes);
            Assert.Contains(typeof(Dog).GUID, foundTypes);
        }

    }
}
