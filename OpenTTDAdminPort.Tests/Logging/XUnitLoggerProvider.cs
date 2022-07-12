using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace OpenTTDAdminPort.Tests.Logging
{
    internal class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly LoggerExternalScopeProvider scopeProvider = new LoggerExternalScopeProvider();

        public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(testOutputHelper, scopeProvider, categoryName);
        }

        public void Dispose()
        {
        }
    }
}
