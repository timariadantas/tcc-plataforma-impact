using ProductService.Domain;
using ProductService.DTO.Requests;
using ProductService.Service;
using ProductService.Tests.Factories;
using Xunit;
using ProductService.Logging;

namespace ProductService.Tests.Unit
{
    public class ProductServiceTests
    {
        private readonly ProductsService _service;

        public ProductServiceTests()
        {
            //  Usa o storage em memória (não banco real)
            var factory = new ProductFactoryInMemory();

            //  Logger simples (poderia ser fake/mocado em testes)
            var logger = new ProductService.Logging.LoggerService();

            logger.Log("Iniciando testes de unidade de ProductService", LogLevel.INFO);

            //  Instancia o serviço de produtos com storage + logger
            _service = new ProductsService(factory, logger);
        }

        [Fact]
        public void CreateProduct_ShouldAddProduct()
        {
            // 1. Criamos um produto do Domain
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };

            // 2. Chamamos o service para criar o produto
            _service.Create(product);

            // 3. Verificamos se o produto foi adicionado no storage
            var allProducts = _service.GetAll();

            // Assert: só existe 1 produto
            Assert.Single(allProducts);

            // Assert: o nome é o que definimos
            Assert.Equal("Produto Teste", allProducts[0].Name);
        }

        [Fact]
        public void UpdateProduct_ShouldModifyNameAndDescription()
        {
            // Criar produto
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };
            _service.Create(product);

            // Atualizar nome e descrição usando Service
            var updated = _service.UpdateProd(product.Id, "Nome Atualizado", "Descrição Atualizada");

            // Assert: verifica se os dados foram alterados corretamente
            Assert.Equal("Nome Atualizado", updated.Name);
            Assert.Equal("Descrição Atualizada", updated.Description);
        }

        [Fact]
        public void UpdatePrice_ShouldChangePrice()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };
            _service.Create(product);

            // Atualiza preço
            var updated = _service.UpdatePrice(product.Id, 20);

            // Assert: preço atualizado
            Assert.Equal(20, updated.Price);
        }

        [Fact]
        public void UpdateQuantity_ShouldChangeQuantity()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };
            _service.Create(product);

            // Atualiza quantidade
            var updated = _service.UpdateQuantity(product.Id, 15);

            // Assert: quantidade atualizada corretamente
            Assert.Equal(15, updated.Quantity);
        }

        [Fact]
        public void InactivateProduct_ShouldSetActiveFalse()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };
            _service.Create(product);

            // Inativa produto
            _service.Inactivate(product.Id);

            // Busca produto atualizado
            var updated = _service.GetById(product.Id);

            // Assert: produto agora está inativo
            Assert.False(updated!.Active);
        }

        [Fact]
        public void DeleteProduct_ShouldRemoveProduct()
        {
            var product = new Product
            {
                Name = "Produto Teste",
                Description = "Descrição teste",
                Price = 10.5m,
                Quantity = 5
            };
            _service.Create(product);

            // Deleta produto
            _service.Delete(product.Id);

            // Busca produto deletado
            var deleted = _service.GetById(product.Id);

            // Assert: produto removido, retorna null
            Assert.Null(deleted);
        }

        // TESTES VALIDAÇÃO DE BORDA
         [Fact]
        public void CreateProduct_WithEmptyName_ShouldThrowException()
        {
            var product = new Product
            {
                Name = "", // inválido
                Description = "Desc",
                Price = 10,
                Quantity = 1
            };

            Assert.Throws<ArgumentException>(() => _service.Create(product));
        }

        [Fact]
        public void UpdatePrice_WithZeroOrNegative_ShouldThrowException()
        {
            var product = new Product
            {
                Name = "Produto",
                Description = "Desc",
                Price = 10,
                Quantity = 1
            };

            _service.Create(product);

            Assert.Throws<Exception>(() => _service.UpdatePrice(product.Id, -5));
            Assert.Throws<Exception>(() => _service.UpdatePrice(product.Id, 0));
        }

        [Fact]
        public void UpdateQuantity_WithNegative_ShouldThrowException()
        {
            var product = new Product
            {
                Name = "Produto",
                Description = "Desc",
                Price = 10,
                Quantity = 1
            };

            _service.Create(product);

            Assert.Throws<Exception>(() => _service.UpdateQuantity(product.Id, -10));
        }

        [Fact]
        public void Operations_WithNonExistentId_ShouldThrowException()
        {
            // Testa Update, Delete, Inactivate com ID inválido
            var invalidId = "ID_INVALIDO";

            Assert.Throws<Exception>(() => _service.UpdateProd(invalidId, "x", "y"));
            Assert.Throws<Exception>(() => _service.UpdatePrice(invalidId, 10));
            Assert.Throws<Exception>(() => _service.UpdateQuantity(invalidId, 5));
            Assert.Throws<Exception>(() => _service.Inactivate(invalidId));
            Assert.Throws<Exception>(() => _service.Delete(invalidId));
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            var product = _service.GetById("ID_INVALIDO");
            Assert.Null(product);
        }
    }
}


