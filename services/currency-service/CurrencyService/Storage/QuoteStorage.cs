using System.Collections.Concurrent;
using System.Text.Json;
using CurrencyService.Domain;
using Microsoft.Extensions.Logging;

namespace CurrencyService.Storage;

public class QuoteStorage : IQuoteStorage
{
    private readonly string _apiUrl;
    private readonly ConcurrentDictionary<string, Quote> _cache = new();
    private DateOnly _lastUpdate = DateOnly.MinValue;
    private readonly HttpClient _http = new();
    private readonly ILogger<QuoteStorage> _logger;

    public QuoteStorage(string apiUrl, ILogger<QuoteStorage> logger)
    {
        _apiUrl = apiUrl;
        _logger = logger;
    }

    public async Task<IEnumerable<Quote>> GetAllAsync()
    {
        await EnsureCacheAsync();
        _logger.LogInformation("Retornando {Count} cotações do cache", _cache.Count);
        return _cache.Values;
    }

    public async Task<Quote?> GetByCodeAsync(string code)
    {
        await EnsureCacheAsync();
        _cache.TryGetValue(code.ToUpperInvariant(), out var q);
        _logger.LogInformation(q != null
            ? "Cotação encontrada para {Code}: {Value}"
            : "Cotação {Code} não encontrada", code.ToUpper(), q?.Value);
        return q;
    }

    public async Task RefreshAsync()
    {
        _logger.LogInformation("Forçando atualização do cache via RefreshAsync()");
        await FetchAndFillCache();
    }

    private async Task EnsureCacheAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_lastUpdate == today)
        {
            _logger.LogInformation("Usando cache atualizado em {Date}", _lastUpdate);
            return;
        }
        _logger.LogInformation("Cache expirado. Buscando novas cotações...");
        await FetchAndFillCache();
    }

    private async Task FetchAndFillCache()
    {
        try
        {
            _logger.LogInformation("Consumindo API externa: {Url}", _apiUrl);
            var resp = await _http.GetStringAsync(_apiUrl);
            using var doc = JsonDocument.Parse(resp);
            var root = doc.RootElement;
            _cache.Clear();

            foreach (var prop in root.EnumerateObject())
            {
                if (!prop.Value.TryGetProperty("bid", out var bidEl)) continue;
                var bid = decimal.Parse(bidEl.GetString()!, System.Globalization.CultureInfo.InvariantCulture);
                var quote = new Quote { Code = prop.Name[..3], Value = bid, CreatedAt = DateTime.UtcNow };
                _cache[quote.Code] = quote;
            }

            _lastUpdate = DateOnly.FromDateTime(DateTime.UtcNow);
            _logger.LogInformation("Cache atualizado com {Count} cotações às {Time}", _cache.Count, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cotações na API externa");
        }
    }
}
