using ProductService.Domain;
using System.Collections.Generic;


namespace ProductService.Storage
{
    public interface IProductStorage
    {
        void Create(Product product);
        List<Product> GetAll();
        Product? GetById(string id);
        void Delete(string id);
        void UpdateProd(string id, string name, string description);
        void UpdatePrice(string id, decimal price);
        void UpdateQuantity(string id, int quantity);
        void Inactivate(string id);
    }
}
