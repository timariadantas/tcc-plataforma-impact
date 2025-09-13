using CurrencyService.Domain;

namespace CurrencyService.Storage;

public interface IQuoteStorage
{
    Task<IEnumerable<Quote>> GetAllAsync();
    Task<Quote?> GetByCodeAsync(string code);
    Task RefreshAsync(); // Força atualização da API externa
}
