using CurrencyService.Domain;
using CurrencyService.Storage;

namespace CurrencyService.Service;

public class QuoteService
{
    private readonly IQuoteStorage _storage;

    public QuoteService(IQuoteStorage storage)
    {
        _storage = storage;
    }

    // Retorna todas as cotações
    public async Task<IEnumerable<Quote>> GetQuotesAsync()
    {
        return await _storage.GetAllAsync();
    }

    // Retorna uma cotação específica
    public async Task<Quote?> GetQuoteAsync(string code)
    {
        return await _storage.GetByCodeAsync(code);
    }

    // Forçar atualização manual do cache
    public async Task RefreshAsync()
    {
        await _storage.RefreshAsync();
    }
}
