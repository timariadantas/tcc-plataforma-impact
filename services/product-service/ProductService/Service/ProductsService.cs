using ProductService.Domain; 
using ProductService.Logging; 
using ProductService.Storage; 
using ProductService.Domain.Validation; 
using System;
using System.Collections.Generic;

namespace ProductService.Service
{
    public class ProductsService : IProductsService
    {
        private readonly IProductStorage _storage; // Armazenamento (CRUD)
        private readonly LoggerService _logger; // Para registrar ações
        private readonly List<IValidationStrategy<Product>> _validators; // Lista de validações

        public ProductsService(IProductStorage storage, LoggerService logger)
        {
            _storage = storage;
            _logger = logger;

            // Inicializa validações (nome e preço)
            _validators = new List<IValidationStrategy<Product>>
            {
                new ProductNameValidation(),
                new ProductPriceValidation()
            };
        }

        // Criar produto
        public Product Create(Product product)
        {
            // Executa validações antes de criar
            foreach (var v in _validators)
                v.Validate(product);

            // Salva produto no storage
            _storage.Create(product);

            // Log de criação
            _logger.Log($"Produto criado: {product.Name}");

            // Retorna o produto criado (Domain)
            return product;
        }

        // Retorna todos os produtos
        public List<Product> GetAll() => _storage.GetAll();

        // Buscar produto por ID
        public Product? GetById(string id) => _storage.GetById(id);

        // Atualizar nome e descrição
        public Product UpdateProd(string id, string name, string description)
        {
            // Busca produto no storage
            var product = _storage.GetById(id) ?? throw new Exception("Produto não encontrado");

            // Atualiza nome e descrição
            _storage.UpdateProd(id, name, description);

            // Recarrega produto atualizado
            var updated = _storage.GetById(id)!;

            // Log de atualização
            _logger.Log($"Produto atualizado: {id}, nome: {name}, descrição: {description}");

            return updated;
        }

        // Atualizar preço
        public Product UpdatePrice(string id, decimal price)
        {
            if (price <= 0) throw new Exception("Preço deve ser maior que zero");

            var product = _storage.GetById(id) ?? throw new Exception("Produto não encontrado");

            _storage.UpdatePrice(id, price);

            var updated = _storage.GetById(id)!;

            _logger.Log($"Preço atualizado: {id} -> {price}");

            return updated;
        }

        // Atualizar quantidade
        public Product UpdateQuantity(string id, int quantity)
        {
            if (quantity < 0) throw new Exception("Quantidade inválida");

            var product = _storage.GetById(id) ?? throw new Exception("Produto não encontrado");

            _storage.UpdateQuantity(id, quantity);

            var updated = _storage.GetById(id)!;

            _logger.Log($"Quantidade atualizada: {id} -> {quantity}");

            return updated;
        }

        // Inativar produto
        public void Inactivate(string id)
        {
            var product = _storage.GetById(id) ?? throw new Exception("Produto não encontrado");

            _storage.Inactivate(id);
            _logger.Log($"Produto inativado: {id}");
        }

        // Remover produto permanentemente
        public void Delete(string id)
        {
            var product = _storage.GetById(id) ?? throw new Exception("Produto não encontrado");

            _storage.Delete(id);
            _logger.Log($"Produto removido permanentemente: {id}");
        }
    }
}

// Service trabalha somente com objetos de domínio (Product).