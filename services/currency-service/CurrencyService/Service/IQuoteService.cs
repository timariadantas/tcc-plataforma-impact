using System.Collections.Generic;
using CurrencyService.Domain;

namespace CurrencyService.Service
{
    public interface IQuoteService
    {
        IEnumerable<Quote> GetAll(); // Retorna todas as cotações
        Quote? GetByCode(string code); // Retorna uma cotação específica
    }
}
