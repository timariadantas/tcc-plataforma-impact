using System.Collections.Generic;
using CurrencyService.Domain;
using CurrencyService.Storage;

namespace CurrencyService.Service
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteStorage _storage;

        public QuoteService(IQuoteStorage storage)
        {
            _storage = storage; // Injeta o Storage
        }

        public IEnumerable<Quote> GetAll() => _storage.GetAll(); // Apenas repassa para o storage

        public Quote? GetByCode(string code) => _storage.GetByCode(code); // Repassa para o storage
    }
}
