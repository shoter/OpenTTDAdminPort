using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTTDAdminPort.Assemblies;

using Xunit;

namespace OpenTTDAdminPort.Tests.Assemblies
{
    public class ClassTypeMatcherShould
    {
        private ClassTypeMatcher matcher = new ClassTypeMatcher();

        private interface ISomeInt
        {
        }

        private class SomeClass
        {
        }

        private struct SomeStruct
        {
        }

        [Fact]
        public void NotMatchInterface() => Assert.False(matcher.IsMatching(typeof(ISomeInt)));

        [Fact]
        public void MatchClass() => Assert.True(matcher.IsMatching(typeof(SomeClass)));

        [Fact]
        public void NotMatchStruct() => Assert.False(matcher.IsMatching(typeof(SomeStruct)));
    }
}
