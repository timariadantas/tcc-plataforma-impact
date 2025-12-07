using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CartService.Logging
{
    public class LoggerService : ILogging, IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();// Uma fila thread-safe que armazena mensagens antes de escrever no arquivo/console.
        private readonly string? _logFilePath; // Caminho do arquivo onde os logs serão gravados (opcional).
        private readonly CancellationTokenSource _cts = new(); // Permite cancelar a execução da tarefa de log.
        private readonly Task _logTask; // A tarefa em background que processa os logs.

        //  Construtor para logs apenas em memória (Docker)
        public LoggerService()
        {
            _logTask = Task.Run(ProcessQueueAsync);
        }

        //  Construtor para logs em arquivo + memória (modo local)
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

        /// MÉTODOS PUBLICO DE LOG

        public void LogInformation(string message) =>
        EnqueueLog("INFO", message);

        public void LogWarning(string message) =>
        EnqueueLog("WARN", message);

        public void LogError(string message) =>
        EnqueueLog("ERROR", message);

        public void LogError(string message, Exception ex) =>
        EnqueueLog("ERROR", $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}");

        public void LogDebug(string message) =>
        EnqueueLog("DEBUG", message);

        public void LogCritical(string message) =>
        EnqueueLog("CRITICAL", message);

        //  Método para registrar mensagens
        public void EnqueueLog(string level, string message)
        {
            string formatted = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            _logQueue.Enqueue(formatted);
        }

        // Processa e grava mensagens em arquivo de forma assíncrona
        //Ela roda em loop infinito até receber um cancelamento:
        //lê mensagens da fila  - grava no arquivo ou escreve no console / espera 100ms e repete
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

        //  Fecha o serviço corretamente Quando o serviço é encerrado: / cancela o token /
        //  espera a task terminar /evita perda de logs
        public void Dispose()
        {
            _cts.Cancel();
            _logTask.Wait();
        }
    }
}
