using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CartService.Logging
{
    // LoggerService implementa a interface de log do sistema
    // Ele grava os logs de forma assíncrona usando uma fila interna
    public class LoggerService : ILoggerService, IDisposable
    {
       
        // Fila thread-safe onde as mensagens de log são armazenadas
    
        private readonly ConcurrentQueue<string> _logQueue = new();

        // Caminho do arquivo onde os logs serão gravados (pode ser null → log no console)
        private readonly string? _logFilePath;

        // Token para cancelar a task de processamento quando a aplicação encerrar
        private readonly CancellationTokenSource _cts = new();

        // A task em background que fica processando a fila de logs
        private readonly Task _logTask;


    
        //Construtor usado quando NÃO queremos gravar arquivo
        // (ex: Docker)
       
        public LoggerService()
        {
            // Inicia a task que processa a fila continuamente
            _logTask = Task.Run(ProcessQueueAsync);
        }


     
        // Construtor que grava logs no arquivo de texto
        
        public LoggerService(string logFilePath)
        {
            _logFilePath = logFilePath;

            // Cria o diretório do log se não existir
            var dir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Limpa o arquivo no início da execução (opcional)
            File.WriteAllText(_logFilePath, "");

            // Tarefa que ficará escrevendo os logs no arquivo
            _logTask = Task.Run(ProcessQueueAsync);
        }


 
        //MÉTODOS DA INTERFACE DE LOG
       

        // Log informativo → fluxo normal
        public void LogInformation(string message, params object[] args) =>
            Enqueue("INFO", Format(message, args));

        // Aviso → algo inesperado, mas não é erro
        public void LogWarning(string message, params object[] args) =>
            Enqueue("WARN", Format(message, args));

        // Debug → usado em desenvolvimento
        public void LogDebug(string message, params object[] args) =>
            Enqueue("DEBUG", Format(message, args));

        // Erro simples
        public void LogError(string message, params object[] args) =>
            Enqueue("ERROR", Format(message, args));

        // Erro com Exception → guarda mensagem + stacktrace
        public void LogError(Exception ex, string message, params object[] args) =>
            Enqueue("ERROR", $"{Format(message, args)} | EXCEPTION: {ex.Message} | STACK: {ex.StackTrace}");

        // Erro crítico → algo grave no sistema
        public void LogCritical(string message, params object[] args) =>
            Enqueue("CRITICAL", Format(message, args));


      
        // INFRA E SUPORTE

        // Aplica o string.Format se houver placeholders {0}, {1}...
        private string Format(string message, object[] args)
        {
            if (args == null || args.Length == 0)
                return message;

            return string.Format(message, args);
        }

        // Enfileira a mensagem formatada para ser processada pela task em background
        private void Enqueue(string level, string message)
        {
            string formatted =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            // Adiciona na fila (thread-safe)
            _logQueue.Enqueue(formatted);
        }


 
        //  PROCESSADOR DA FILA DE LOGS
        // Essa função roda em loop infinito até ser cancelada.
        // Enquanto houver mensagens na fila → ela escreve no arquivo
        // ou no console, dependendo da configuração.
        //
        private async Task ProcessQueueAsync()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                // Processa todas as mensagens disponíveis no momento
                while (_logQueue.TryDequeue(out var logMessage))
                {
                    if (_logFilePath != null)
                    {
                        // Grava no arquivo
                        await File.AppendAllTextAsync(_logFilePath, logMessage + Environment.NewLine);
                    }
                    else
                    {
                        // Exibe no console
                        Console.WriteLine(logMessage);
                    }
                }

                // Pausa leve para evitar uso excessivo da CPU
                await Task.Delay(100);
            }
        }


    
        //FINALIZAÇÃO / FECHAMENTO DO LOGGER
        // Quando sua aplicação fechar, chamará Dispose()
        // Isso garante:
        //      • Cancela a task de log
        //      • Espera o restante das mensagens serem escritas
        //      • Evita perda de log
        //
        public void Dispose()
        {
            _cts.Cancel();      // pede para parar o loop
            _logTask.Wait();    // espera a task terminar
        }
    }
}
