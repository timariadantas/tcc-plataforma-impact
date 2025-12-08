using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProductService.Logging
{
    public class LoggerService : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string? _logFilePath;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _logTask;

        public LoggerService()
        {
            _logTask = Task.Run(ProcessQueueAsync);
        }

        public LoggerService(string logFilePath)
        {
            _logFilePath = logFilePath;

            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            File.WriteAllText(_logFilePath, ""); // cria ou limpa o arquivo
            _logTask = Task.Run(ProcessQueueAsync);
        }

        // Agora recebe o LogLevel
        public void Log(string message, LogLevel level = LogLevel.INFO)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            _logQueue.Enqueue(timestampedMessage);
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                while (_logQueue.TryDequeue(out var logMessage))
                {
                    if (!string.IsNullOrEmpty(_logFilePath))
                        await File.AppendAllTextAsync(_logFilePath, logMessage + Environment.NewLine);
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
