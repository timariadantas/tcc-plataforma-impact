using ProductService.Domain;
using System.Collections.Generic;

namespace ProductService.Service
{
    public interface IProductsService
    {
        void Create(Product product);
        Product? GetById(string id);
        List<Product> GetAll();
        void Delete(string id);
         void UpdateProd(string id, string name, string desc);
        void UpdatePrice(string id, decimal price);
        void UpdateQuantity(string id, int quantity);
        void Inactivate(string id);
    }
}
