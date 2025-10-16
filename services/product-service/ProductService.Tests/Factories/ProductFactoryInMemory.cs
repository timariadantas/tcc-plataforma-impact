using ProductService.Domain;
using ProductService.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using NUlid;

// // Factory fake/in-memory para testes
    // Substitui o storage real, não precisa de banco de dados

namespace ProductService.Tests.Factories

{
    public class ProductFactoryInMemory : IProductStorage
    {
        private readonly List<Product> _products = new();

        public void Create(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            product.Id = Ulid.NewUlid().ToString();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            _products.Add(product);
        }

        public List<Product> GetAll()
        {
            return _products.ToList();
        }

        public Product? GetById(string id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Delete(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
                _products.Remove(product);
        }

        public void UpdateProd(string id, string name, string description)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Name = name;
                product.Description = description;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdatePrice(string id, decimal price)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Price = price;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateQuantity(string id, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Quantity = quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Inactivate(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Active = false;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

