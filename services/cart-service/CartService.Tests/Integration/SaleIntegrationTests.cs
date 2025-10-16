using Xunit;
using CartService.Storage;
using CartService.Domain;
using CartService.Service;
using CartService.Logging;
using DotNetEnv;
using System;
using System.Linq;

namespace CartService.Tests.Integration
{
    public class SaleIntegrationTests : IDisposable
    {
        private readonly SalesService _service;
        private readonly ISalesStorage _storage;
        private readonly LoggerService _logger;

        public SaleIntegrationTests()
        {
            // 🔹 Carrega variáveis do .env
            Env.Load("/home/mariadantas/plataforma-tcc/.env");

            // 🔹 Detecta se está rodando dentro do Docker
            bool runningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            // 🔹 Configura host e porta dinamicamente
            string host = runningInDocker ? "cart-db" : Environment.GetEnvironmentVariable("CART_DB_HOST");
            string port = runningInDocker ? "5432" : Environment.GetEnvironmentVariable("CART_DB_PORT");
            string user = Environment.GetEnvironmentVariable("CART_DB_USER");
            string password = Environment.GetEnvironmentVariable("CART_DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("CART_DB_NAME");

            if (host == null || port == null || user == null || password == null || database == null)
                throw new Exception("Variáveis de ambiente do banco não carregadas corretamente");

            // 🔹 Monta a connection string
            var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

            // 🔹 Instancia Storage e Service
            _storage = new SalesStorage(connectionString);
            _service = new SalesService(_storage);

            // 🔹 Instancia Logger
            _logger = new LoggerService("logs/cart-integration-tests.log");
        }

        [Fact]
        public void CreateSale_ShouldPersistInDatabase()
        {
            _logger.Log("Iniciando teste CreateSale_ShouldPersistInDatabase");

            var sale = _service.CreateSale("client-123");
            var retrieved = _service.GetSaleById(sale.Id);

            Assert.NotNull(retrieved);
            Assert.Equal("client-123", retrieved.ClientId);
            Assert.Equal((int)SaleStatus.Started, retrieved.Status);

            _logger.Log($"Venda criada com sucesso: {sale.Id}");
        }

        [Fact]
        public void AddItem_ShouldPersistItemInSale()
        {
            _logger.Log("Iniciando teste AddItem_ShouldPersistItemInSale");

            var sale = _service.CreateSale("client-123");
            _service.AddItem(sale.Id, "product-1", 2);

            var items = _storage.GetItemsBySaleId(sale.Id);
            Assert.Single(items);
            Assert.Equal("product-1", items.First().ProductId);
            Assert.Equal(2, items.First().Quantity);

            _logger.Log($"Item adicionado à venda: {sale.Id}, Produto: product-1, Quantidade: 2");
        }

        [Fact]
        public void UpdateItemQuantity_ShouldModifyQuantity()
        {
            _logger.Log("Iniciando teste UpdateItemQuantity_ShouldModifyQuantity");

            var sale = _service.CreateSale("client-123");
            _service.AddItem(sale.Id, "product-1", 2);

            var item = _storage.GetItemsBySaleId(sale.Id).First();
            _service.UpdateItemQuantity(item.Id, 5);

            var updatedItem = _storage.GetItemsBySaleId(sale.Id).First();
            Assert.Equal(5, updatedItem.Quantity);

            _logger.Log($"Quantidade atualizada para item {item.Id}: 5");
        }

        [Fact]
        public void CancelSale_ShouldUpdateStatus()
        {
            _logger.Log("Iniciando teste CancelSale_ShouldUpdateStatus");

            var sale = _service.CreateSale("client-123");
            _service.CancelSale(sale.Id);

            var updated = _service.GetSaleById(sale.Id);
            Assert.Equal((int)SaleStatus.Canceled, updated.Status);

            _logger.Log($"Venda cancelada com sucesso: {sale.Id}");
        }

        public void Dispose()
        {
            _logger.Dispose();
        }
    }
}

