using AutoFixture;

namespace OpenTTDAdminPort.Tests.Cusomizations
{
    public class DefaultFixture : Fixture
    {
        public DefaultFixture()
        {
            Customize(new StateCusomizations());
        }
    }
}