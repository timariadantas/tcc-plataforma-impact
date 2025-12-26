using Xunit;
using CartService.Storage;
using CartService.Service;
using CartService.Logging;
using CartService.Domain;
using CartService.DTO.Requests;
using DotNetEnv;
using System;
using CartService.Tests.Factories;
namespace CartService.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class SaleIntegrationTests : IDisposable
    {
        private readonly SalesService _service;
        private readonly SalesStorage _storage;
        private readonly ILoggerService _logger;
        

        public SaleIntegrationTests()
        {
            // Carrega variáveis do ambiente
            DotNetEnv.Env.Load("../../../../.env");

            var dbHost = Environment.GetEnvironmentVariable("CART_DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("CART_DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("CART_DB_USER");
            var dbPass = Environment.GetEnvironmentVariable("CART_DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("CART_DB_NAME");

            var connectionString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPass};Database={dbName}";

            _logger = new LoggerFake();
            _storage = new SalesStorage(connectionString);
            _service = new SalesService(_storage, _logger);
        }

        [Fact]
        public void CreateSale_ShouldPersistInDatabase()
        {
            var sale = _service.CreateSale("client_test");

            Assert.NotNull(sale);
            Assert.Equal("client_test", sale.ClientId);
            Assert.Equal((int)SaleStatus.Started, sale.Status);
        }

        [Fact]
        public void AddItem_ShouldInsertItemInDatabase()
        {
            var sale = _service.CreateSale("client_test");
            _service.AddItem(sale.Id, "product_1", 2);

            var reloaded = _service.GetSaleById(sale.Id);
            Assert.NotNull(reloaded);
            Assert.Single(reloaded!.Items);
            Assert.Equal("product_1", reloaded.Items[0].ProductId);
        }

        [Fact]
        public void GetSaleById_ShouldReturnCorrectSale()
        {
            var sale = _service.CreateSale("client_test");
            var found = _service.GetSaleById(sale.Id);

            Assert.NotNull(found);
            Assert.Equal(sale.Id, found!.Id);
            Assert.Equal(sale.ClientId, found.ClientId);
        }

        [Fact]
        public void CancelSale_ShouldUpdateStatus()
        {
            var sale = _service.CreateSale("client_test");
            _service.CancelSale(sale.Id);

            var canceled = _service.GetSaleById(sale.Id);
            Assert.Equal((int)SaleStatus.Canceled, canceled!.Status);
        }

        public void Dispose()
        {
            // Cleanup opcional
        }
    }
}
