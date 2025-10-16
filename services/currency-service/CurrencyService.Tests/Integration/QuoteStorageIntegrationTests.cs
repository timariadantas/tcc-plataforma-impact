using System;
using System.Linq;
using CurrencyService.Storage;
using CurrencyService.Logging;
using Xunit;
using FluentAssertions;

namespace CurrencyService.Tests.Integration
{
    public class QuoteStorageIntegrationTests
    {
        private readonly QuoteStorage _storage;

        public QuoteStorageIntegrationTests()
        {
            var logger = new LoggerService(); 
            _storage = new QuoteStorage("https://economia.awesomeapi.com.br/all", logger);
        }

        [Fact]
        public void GetAll_ShouldReturnSomeQuotes()
        {
            var quotes = _storage.GetAll();
            quotes.Should().NotBeEmpty();
        }

        [Fact]
        public void GetByCode_ShouldReturnUsdQuote()
        {
            var quote = _storage.GetByCode("USD");
            quote.Should().NotBeNull();
            quote!.Code.Should().Be("USD");
            quote.Value.Should().BeGreaterThan(0);
        }
    }

}


