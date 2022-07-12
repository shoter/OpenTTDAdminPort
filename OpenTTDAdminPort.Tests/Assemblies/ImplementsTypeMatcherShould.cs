using System;
using System.Collections.Generic;
using System.Linq;

using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Tests.Assemblies.TestTypes;

using Xunit;

namespace OpenTTDAdminPort.Tests.Assemblies
{
    public class ImplementsTypeMatcherShould
    {
        private readonly List<Type> inputTypes = new()
        {
            typeof(Cat),
            typeof(Dog),
            typeof(IAlienAnimal),
            typeof(IFurniture),
            typeof(Table),
            typeof(Chair),
            typeof(IAnimal),
            typeof(IFurnitureBox<>),
        };

        [Fact]
        public void FindOnlyTypesThatAreImplementingGivenInterface()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IAnimal));

            Type[] expectedTypes = new Type[] { typeof(Cat), typeof(Dog), typeof(IAlienAnimal) };

            AssertTest(matcher, expectedTypes);
        }

        [Fact]
        public void BeAbleToFindGenericTypeWithArgumentThatIsAlsoGeneric_WhenInterfaceOfGenericArgumentWasPassed()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IFurnitureBox<>), typeof(IFurniture));

            Type[] expectedTypes = new Type[] { typeof(GenericFurnitureBox<>) };
            inputTypes.Add(typeof(GenericFurnitureBox<>));

            AssertTest(matcher, expectedTypes);
        }

        [Fact]
        public void BeAbleToMatchGenericParameterWhichIsConcreteType_WhenInterfaceOfThisConcreteTypeWasPassed()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IFurnitureBox<>), typeof(IFurniture));

            Type[] expectedTypes = new Type[] { typeof(ChairBox) };
            inputTypes.Add(typeof(ChairBox));

            AssertTest(matcher, expectedTypes);
        }

        [Fact]
        public void BeAbleToMatchGenericParameterWhichIsConcreteType_WhenConcreteTypeWasPassed()
        {
            var matcher = new ImplementsTypeMatcher(typeof(IFurnitureBox<>), typeof(Chair));

            Type[] expectedTypes = new Type[] { typeof(ChairBox) };
            inputTypes.Add(typeof(ChairBox));

            AssertTest(matcher, expectedTypes);
        }

        private void AssertTest(ImplementsTypeMatcher matcher, Type[] expectedTypes)
        {
            foreach (var it in inputTypes)
            {
                if (matcher.IsMatching(it) && !expectedTypes.Contains(it))
                {
                    throw new Exception($"Matcher is matching {it} when it is not on the expected types list");
                }
                else if (!matcher.IsMatching(it) && expectedTypes.Contains(it))
                {
                    throw new Exception($"Matcher is NOT matching {it} when it is on the expected types list");
                }
            }
        }
    }
}
