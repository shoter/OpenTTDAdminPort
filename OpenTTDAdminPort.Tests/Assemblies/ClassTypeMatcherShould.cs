using OpenTTDAdminPort.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Assemblies
{
    public class ClassTypeMatcherShould
    {
        ClassTypeMatcher matcher = new ClassTypeMatcher();
        interface someInt { }

        class someClass { }

        struct someStruct { }

        [Fact]
        public void NotMatchInterface() => Assert.False(matcher.IsMatching(typeof(someInt)));

        [Fact]
        public void MatchClass() => Assert.True(matcher.IsMatching(typeof(someClass)));
        [Fact]
        public void NotMatchStruct() => Assert.False(matcher.IsMatching(typeof(someStruct)));
    }
}
