using System.Collections.Generic;
using CurrencyService.Domain;

namespace CurrencyService.Storage
{
    public interface IQuoteStorage
    {
        IEnumerable<Quote> GetAll(); // Retorna todas as cotações
        Quote? GetByCode(string code); // Retorna uma cotação pelo código
    }
}
