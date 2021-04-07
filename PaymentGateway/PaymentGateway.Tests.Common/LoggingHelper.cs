using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Tests.Common
{
    public class LoggingHelper
    {
        private readonly List<(LogLevel Level, string Message)> _messages = new List<(LogLevel, string)>();
        public ILogger<T> CreateLogger<T>() => new Logger<T>(_messages);

        internal class Logger<T> : ILogger<T>
        {
            private readonly List<(LogLevel Level, string Message)> _messages;

            internal Logger(List<(LogLevel Level, string Message)> messages)
            {
                _messages = messages;
            }
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _messages.Add((logLevel, formatter(state, exception)));
            }
        }

        public IEnumerable<string> Information { get => AtLevel(LogLevel.Information); }

        public IEnumerable<string> Warnings { get => AtLevel(LogLevel.Warning); }

        public IEnumerable<string> Errors { get => AtLevel(LogLevel.Error); }

        public IEnumerable<string> AtLevel(LogLevel level) =>
            _messages.Where(x => x.Level == level).Select(x => x.Message);
    }
}
