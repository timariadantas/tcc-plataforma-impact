using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CartService.Logging
{
    public class LoggerService : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string? _logFilePath;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _logTask;

        // 🔹 Construtor para logs apenas em memória (Docker)
        public LoggerService()
        {
            _logTask = Task.Run(ProcessQueueAsync);
        }

        // 🔹 Construtor para logs em arquivo + memória (modo local)
        public LoggerService(string logFilePath)
        {
            _logFilePath = logFilePath;

            // Cria o diretório de log se não existir
            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            // Limpa o arquivo no início
            File.WriteAllText(_logFilePath, "");

            _logTask = Task.Run(ProcessQueueAsync);
        }

        // 🔹 Método para registrar mensagens
        public void Log(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            _logQueue.Enqueue(timestampedMessage);
        }

        // 🔹 Processa e grava mensagens em arquivo de forma assíncrona
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
                    else
                    {
                        Console.WriteLine(logMessage);
                    }
                }
                await Task.Delay(100);
            }
        }

        // 🔹 Fecha o serviço corretamente
        public void Dispose()
        {
            _cts.Cancel();
            _logTask.Wait();
        }
    }
}
