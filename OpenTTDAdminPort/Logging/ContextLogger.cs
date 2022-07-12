using System;

using Microsoft.Extensions.Logging;

namespace OpenTTDAdminPort.Logging
{
    internal class ContextLogger : ILogger
    {
        private readonly ILogger logger;
        private readonly string context;

        public ContextLogger(ILogger logger, string context)
        {
            this.logger = logger;
            this.context = context;
        }

        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, (TState s, Exception e) => $"{context}:{formatter(s, e)}");
        }
    }

    public class ContextLogger<T> : ILogger<T>
    {
        private readonly ILogger<T> logger;
        private readonly string context;

        public ContextLogger(ILogger<T> logger, string context)
        {
            this.logger = logger;
            this.context = context;
        }

        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, (TState s, Exception e) => $"{context}:{formatter(s, e)}");
        }
    }
}
