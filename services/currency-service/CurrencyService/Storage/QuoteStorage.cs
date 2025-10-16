using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using CurrencyService.Domain;
using CurrencyService.Logging;

namespace CurrencyService.Storage
{
    public class QuoteStorage : IQuoteStorage
    {
        private readonly string _apiUrl;
        private readonly Dictionary<string, Quote> _cache = new();
        private DateTime _lastUpdate = DateTime.MinValue;
        private readonly ILoggerService _logger;

        public QuoteStorage(string apiUrl, ILoggerService logger)
        {
            _apiUrl = apiUrl;
            _logger = logger;
        }

        // Retorna todas as cotações do cache
        public IEnumerable<Quote> GetAll()
        {
            _logger.Log("INFO", "GetAll() chamado, garantindo cache...");
            EnsureCache();
            return _cache.Values;
        }

        // Retorna cotação específica
        public Quote? GetByCode(string code)
        {
            _logger.Log("INFO", $"GetByCode() chamado para {code}");
            EnsureCache();
            _cache.TryGetValue(code.ToUpperInvariant(), out var q);
            return q;
        }

        // Atualiza o cache da API
        private void EnsureCache()
        {
            // Para teste, força atualizar a cada chamada
            if (_lastUpdate.Date != DateTime.UtcNow.Date)
            {
                _logger.Log("INFO", "Atualizando cache de cotações...");
                FetchAndFillCache();
                _lastUpdate = DateTime.UtcNow;
            }
        }

        // Busca cotações na API externa
        private void FetchAndFillCache()
        {
            try
            {
                using var http = new HttpClient();

                // User-Agent obrigatório para algumas APIs
                http.DefaultRequestHeaders.Add("User-Agent", "CurrencyServiceClient");

                var response = http.GetAsync(_apiUrl).Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;
                _logger.Log("INFO", $"JSON recebido da API (primeiros 200 chars): {json.Substring(0, Math.Min(200, json.Length))}...");

                using var doc = JsonDocument.Parse(json);
                _cache.Clear();

                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (!prop.Value.TryGetProperty("bid", out var bidEl)) continue;

                    if (!decimal.TryParse(bidEl.GetString(),
                                          System.Globalization.NumberStyles.Any,
                                          System.Globalization.CultureInfo.InvariantCulture,
                                          out var bid))
                    {
                        _logger.Log("WARNING", $"Não foi possível converter bid da moeda {prop.Name}");
                        continue;
                    }

                    var quote = new Quote
                    {
                        Code = prop.Name,
                        Value = bid,
                        CreatedAt = DateTime.UtcNow
                    };

                    _cache[quote.Code] = quote;
                    _logger.Log("INFO", $"Cotação {quote.Code} atualizada: {quote.Value}");
                }

                _logger.Log("INFO", $"Cache atualizado com {_cache.Count} cotações");
            }
            catch (Exception ex)
            {
                _logger.Log("ERROR", $"Erro ao atualizar cache de cotações: {ex}");
            }
        }
    }
}
