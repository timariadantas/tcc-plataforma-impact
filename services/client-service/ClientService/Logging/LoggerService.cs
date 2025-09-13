using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClientService.Logging
{
    public class LoggerService : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string? _logFilePath;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _logTask;

        // Construtor para logs apenas em memória (Docker)
        public LoggerService()
        {
            _logTask = Task.Run(ProcessQueueAsync);
        }

        // Construtor para logs em arquivo + memória (local)
        public LoggerService(string logFilePath)
        {
            _logFilePath = logFilePath;

            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            File.WriteAllText(_logFilePath, "");

            _logTask = Task.Run(ProcessQueueAsync);
        }

        public void Log(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            _logQueue.Enqueue(timestampedMessage);

            // Se estiver usando arquivo, ele será gravado pela Task
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                while (_logQueue.TryDequeue(out var logMessage))
                {
                    if (!string.IsNullOrEmpty(_logFilePath))
                    {
                        await File.AppendAllTextAsync(_logFilePath, logMessage + Environment.NewLine);
                    }
                }
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _logTask.Wait();
        }
    }
}
