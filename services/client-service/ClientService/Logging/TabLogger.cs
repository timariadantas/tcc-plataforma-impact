using System.Text;

namespace ClientService.Logging
{
    public class TabLogger
    {
        private readonly string _filePath;
        private static readonly object _lock = new(); // Lock para escrita thread-safe.

        public TabLogger(string filePath)  
        {
            _filePath = filePath;
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        private void Write(string level, string message)
        {
            var line = $"{DateTime.UtcNow:0} \t{level}\t{message}\n";
            lock (_lock)
            {
                File.AppendAllText(_filePath, line, Encoding.UTF8);
            }
            Console.Write(line);
        }
        public void Debug(string msg) => Write("DEBUD", msg);
        public void Info(string msg) => Write("INFO", msg);
        public void Warning(string msg) => Write("WARNING", msg);
        public void Error(string msg) => Write("ERROR", msg);
        public void Critical(string msg) => Write("CRITICAL", msg);
    }
}
// Logger simples que grava linhas no formato
// Classe TabLogger, que será responsável por gravar logs em arquivo e exibir no console.
// Objeto lock - usado para bloquear múltiplas threads tentando escrever no arquivo ao mesmo tempo, evitando conflito de escrita.
// O Construtor do logger: recebe onde salvar o arquivo de log.
// Directory.CreateDirectory cria a pasta se não existir, evitando erro na hora de escrever.
// Encoding.UTF8 → garante que caracteres especiais sejam salvos corretamente.
// Suporta níveis de log diferentes (Debug, Info, Warning, Error, Critical).