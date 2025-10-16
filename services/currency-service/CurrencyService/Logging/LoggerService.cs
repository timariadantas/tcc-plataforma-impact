using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyService.Logging
{
    public class LoggerService : ILoggerService, IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string? _logFilePath;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _logTask;

        public LoggerService(string? logFilePath = null)
        {
            _logFilePath = logFilePath;
            if (!string.IsNullOrEmpty(_logFilePath))
            {
                var dir = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(_logFilePath, "");
            }

            _logTask = Task.Run(ProcessQueue);
        }

        public void Log(string level, string message)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var line = $"{timestamp}\t{level}\t{message}";
            _logQueue.Enqueue(line);
        }

        private async Task ProcessQueue()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                while (_logQueue.TryDequeue(out var line))
                {
                    if (!string.IsNullOrEmpty(_logFilePath))
                        await File.AppendAllTextAsync(_logFilePath, line + "\n");
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
