namespace CartService.Logging
{
    public interface ILoggerService
    {
        // Mensagens informativas — fluxo normal da aplicação
        void LogInformation(string message, params object[] args);

        // Alertas — algo inesperado, mas não é erro
        void LogWarning(string message, params object[] args);

        // Debug — usado em desenvolvimento
        void LogDebug(string message, params object[] args);

        // Erro sem exception
        void LogError(string message, params object[] args);

        // Erro com exception e mensagem
        void LogError(Exception ex, string message, params object[] args);

        // Erros críticos — derrubam o sistema
        void LogCritical(string message, params object[] args);
    }
}
