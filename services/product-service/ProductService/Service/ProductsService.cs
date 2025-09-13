using ProductService.Domain;
using ProductService.Domain.Validation;
using ProductService.Storage;
using ProductService.Logging;
using System;
using System.Collections.Generic;

namespace ProductService.Service
{
    public class ProductsService : IProductsService
    {
        private readonly IProductStorage _storage;
        private readonly LoggerService _logger;
        private readonly List<IValidationStrategy<Product>> _validators;

        public ProductsService(IProductStorage storage, LoggerService logger)
        {
            _storage = storage;
            _logger = logger;

            _validators = new List<IValidationStrategy<Product>>
            {
                new ProductNameValidation(),
                new ProductPriceValidation()
            };
        }

        public void Create(Product product)
        {
            foreach (var v in _validators) v.Validate(product);

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _storage.Create(product);
            _logger.Log($"Produto criado: {product.Name} ({product.Id})");
        }

        public List<Product> GetAll() => _storage.GetAll();

        public Product? GetById(string id) => _storage.GetById(id);

        public void Delete(string id)
        {
            _storage.Delete(id);
            _logger.Log($"Produto removido: {id}");
        }

        public void UpdateProd(string id, string name, string description)
        {
            var product = _storage.GetById(id);
            if (product == null) throw new Exception("Produto não encontrado");

            _storage.UpdateProd(id, name, description);
            _logger.Log($"Produto atualizado: {id}, nome: {name}, descrição: {description}");
        }

        public void UpdatePrice(string id, decimal price)
        {
            if (price <= 0) throw new Exception("Preço deve ser maior que zero");

            var product = _storage.GetById(id);
            if (product == null) throw new Exception("Produto não encontrado");

            _storage.UpdatePrice(id, price);
            _logger.Log($"Preço do produto atualizado: {id}, novo preço: {price}");
        }

        public void UpdateQuantity(string id, int quantity)
        {
            if (quantity < 0) throw new Exception("Quantidade inválida");

            var product = _storage.GetById(id);
            if (product == null) throw new Exception("Produto não encontrado");

            _storage.UpdateQuantity(id, quantity);
            _logger.Log($"Quantidade do produto atualizada: {id}, nova quantidade: {quantity}");
        }

        public void Inactivate(string id)
        {
            var product = _storage.GetById(id);
            if (product == null) throw new Exception("Produto não encontrado");

            _storage.Inactivate(id);
            _logger.Log($"Produto inativado: {id}");
        }
    }
}
