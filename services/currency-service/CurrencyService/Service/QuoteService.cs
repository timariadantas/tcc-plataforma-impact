using CurrencyService.Domain;
using CurrencyService.Storage;
using Microsoft.Extensions.Logging;

namespace CurrencyService.Service;

public class QuoteService
{
    private readonly IQuoteStorage _storage;
    private readonly ILogger<QuoteService> _logger;

    public QuoteService(IQuoteStorage storage, ILogger<QuoteService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public async Task<IEnumerable<Quote>> GetQuotesAsync()
    {
        _logger.LogInformation("Service: Obtendo todas as cotações");
        return await _storage.GetAllAsync();
    }

    public async Task<Quote?> GetQuoteAsync(string code)
    {
        _logger.LogInformation("Service: Obtendo cotação para {Code}", code);
        return await _storage.GetByCodeAsync(code);
    }

    public async Task RefreshAsync()
    {
        _logger.LogInformation("Service: Forçando atualização de cache");
        await _storage.RefreshAsync();
    }
}
