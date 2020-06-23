using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Tests.Assemblies.TestTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Assemblies
{
    public class ImplementsTypeMatcherShould
    {
        Type[] inputTypes = new Type[] { typeof(Cat), typeof(Dog), typeof(IAlienAnimal), typeof(IFurniture), typeof(Table), typeof(Chair), typeof(IAnimal), typeof(IFurnitureBox<>), typeof(GenericFurnitureBox<>) };

        [Fact]
        public void FindOnlyTypesThatAreImplementingGivenInterface()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IAnimal));

            Type[] expectedTypes = new Type[] { typeof(Cat), typeof(Dog), typeof(IAlienAnimal) };

            foreach(var it in inputTypes)
            {
                if (matcher.IsMatching(it) && !expectedTypes.Contains(it))
                    throw new Exception($"Matcher is matching {it} when it is not on the expected types list");

                if (!matcher.IsMatching(it) && expectedTypes.Contains(it))
                    throw new Exception($"Matcher is NOT matching {it} when it is on the expected types list");

            }
        }

        [Fact]
        public void BeAbleToFindGenericTypeWithArguments_WhenNoArgumentsWerePassed()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IFurnitureBox<>), typeof(IFurniture));

            Type[] expectedTypes = new Type[] { typeof(GenericFurnitureBox<>) };

            foreach (var it in inputTypes)
            {
                if (matcher.IsMatching(it) && !expectedTypes.Contains(it))
                    throw new Exception($"Matcher is matching {it} when it is not on the expected types list");

                if (!matcher.IsMatching(it) && expectedTypes.Contains(it))
                    throw new Exception($"Matcher is NOT matching {it} when it is on the expected types list");

            }
        }

    }
}
