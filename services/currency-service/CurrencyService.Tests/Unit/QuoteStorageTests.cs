using CurrencyService.Domain;
using CurrencyService.Storage;
using FluentAssertions;
using Xunit;
using CurrencyService.Tests.Factories;


namespace CurrencyService.Tests.Unit
{
    public class QuoteStorageTests
    {
        [Fact]
        public void GetByCode_ShouldReturnNull_WhenNotFound()
        {
            var storage = QuoteStorageFactory.Create();

            var result = storage.GetByCode("NON_EXISTENT");

            result.Should().BeNull();
        }

        [Fact]
        public void GetAll_ShouldReturnEmpty_WhenNoQuotes()
        {
            var storage = QuoteStorageFactory.Create();

            var result = storage.GetAll();

            result.Should().BeEmpty();
        }
    }
}
