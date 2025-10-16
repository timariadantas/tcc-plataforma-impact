using Xunit;
using ProductService.Storage;
using ProductService.Domain;
using ProductService.Service;
using ProductService.Logging;

using DotNetEnv;
using System;

namespace ProductService.IntegrationTests
{
    public class ProductIntegrationTests
    {
        private readonly ProductsService _service;
        private readonly string _connectionString; // ✅ campo declarado aqui

        public ProductIntegrationTests()
        {

            // Carrega o .env da raiz do projeto
            Env.Load("/home/mariadantas/plataforma-tcc/.env");

            // Lê variáveis do ambiente
            var host = Environment.GetEnvironmentVariable("PRODUCT_DB_HOST");
            var port = Environment.GetEnvironmentVariable("PRODUCT_DB_PORT");
            var user = Environment.GetEnvironmentVariable("PRODUCT_DB_USER");
            var password = Environment.GetEnvironmentVariable("PRODUCT_DB_PASSWORD");
            var database = Environment.GetEnvironmentVariable("PRODUCT_DB_NAME");


            // Console.WriteLine($"DEBUG: HOST = {host}");
            // Console.WriteLine($"DEBUG: PORT = {port}");
            //Console.WriteLine($"DEBUG: USER = {user}");
            //Console.WriteLine($"DEBUG: PASS = {password}");
            //Console.WriteLine($"DEBUG: DB = {database}");

            // Checa se alguma variável está null
            if (host == null || port == null || user == null || password == null || database == null)
            {
                throw new Exception("Variáveis de ambiente do banco não carregadas corretamente");
            }

            // Monta a connection string
            _connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";
            // 🔹 Cria storage real
            var storage = new ProductStorage(_connectionString);

            // 🔹 Logger simples
            _service = new ProductsService(storage, new LoggerService());

        }

        [Fact]
        public void CreateProduct_ShouldPersistInDatabase()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);
            var products = _service.GetAll();

            Assert.Contains(products, p => p.Name == "Produto Teste");
        }

        [Fact]
        public void UpdateProduct_ShouldModifyNameAndDescription()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);

            var updated = _service.UpdateProd(product.Id, "Nome Atualizado", "Descrição Atualizada");

            Assert.Equal("Nome Atualizado", updated.Name);
            Assert.Equal("Descrição Atualizada", updated.Description);
        }

        [Fact]
        public void UpdatePrice_ShouldChangePrice()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);

            var updated = _service.UpdatePrice(product.Id, 20m);

            Assert.Equal(20m, updated.Price);
        }

        [Fact]
        public void UpdateQuantity_ShouldChangeQuantity()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);

            var updated = _service.UpdateQuantity(product.Id, 15);

            Assert.Equal(15, updated.Quantity);
        }

        [Fact]
        public void InactivateProduct_ShouldSetActiveFalse()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);
            _service.Inactivate(product.Id);

            var updated = _service.GetById(product.Id);

            Assert.False(updated!.Active);
        }

        [Fact]
        public void DeleteProduct_ShouldRemoveProduct()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição Teste",
                Price = 10.5m,
                Quantity = 5
            };

            _service.Create(product);
            _service.Delete(product.Id);

            var deleted = _service.GetById(product.Id);

            Assert.Null(deleted);
        }
    }
}
