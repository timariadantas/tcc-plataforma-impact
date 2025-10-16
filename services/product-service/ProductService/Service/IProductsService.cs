using ProductService.Domain;
using ProductService.Service;
using System.Collections.Generic; 

namespace ProductService.Service
{
    public interface IProductsService
    {
        Product Create(Product product);
        List<Product> GetAll();
        Product? GetById(string id);
        Product UpdateProd(string id, string name, string description);
        Product UpdatePrice(string id, decimal price);
        Product UpdateQuantity(string id, int quantity);
        void Inactivate(string id);
        void Delete(string id);
    }
}
