using System;
using System.Runtime.CompilerServices;
using CartService.Logging;

namespace CartService.Tests.Factories
{
    public class LoggerFake : ILoggerService
    {
        public void LogInformation(string messege, params object[] args)
        {
            // FAKE ...
        }

        public void LogWarning(string message, params object[] args)
        {
            // FAKE ...
        }
        public void LogDebug(string messege, params object[] args)
        {
            // FAKE ...
        }
    
         public void LogError(string message, params object[] args)
        {
            // Fake: não faz nada
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            // Fake: não faz nada
        }

        public void LogCritical (string messege, params object[] args)
        {
            // FAKE ...
        }
    }
}

// Teste não deve logar