using System.Collections.Concurrent;
using System.Text.Json;
using CurrencyService.Domain;

namespace CurrencyService.Storage;

public class QuoteStorage : IQuoteStorage
{
    private readonly string _apiUrl;
    private readonly ConcurrentDictionary<string, Quote> _cache = new();
    private DateOnly _lastUpdate = DateOnly.MinValue;
    private readonly HttpClient _http = new();

    public QuoteStorage(string apiUrl)
    {
        _apiUrl = apiUrl;
    }

    public async Task<IEnumerable<Quote>> GetAllAsync()
    {
        await EnsureCacheAsync();
        return _cache.Values;
    }

    public async Task<Quote?> GetByCodeAsync(string code)
    {
        await EnsureCacheAsync();
        _cache.TryGetValue(code.ToUpperInvariant(), out var q);
        return q;
    }

    public async Task RefreshAsync() => await FetchAndFillCache();

    private async Task EnsureCacheAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (_lastUpdate == today) return;
        await FetchAndFillCache();
    }

    private async Task FetchAndFillCache()
    {
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
    }
}
