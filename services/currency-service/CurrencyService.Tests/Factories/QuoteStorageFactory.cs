using CurrencyService.Logging;
using CurrencyService.Storage;
using CurrencyService.Tests.Factories;
using Moq;

namespace CurrencyService.Tests.Factories
{
    public static class QuoteStorageFactory
    {
        public static QuoteStorage Create(string apiUrl = "http://fakeapi.com")
        {
            // Cria um mock do ILoggerService
            var loggerMock = new Mock<ILoggerService>();
            return new QuoteStorage(apiUrl, loggerMock.Object);
        }
    }
}
