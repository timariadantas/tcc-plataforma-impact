using ProductService.Domain;

namespace ProductService.Domain.Validation
{
    public class ProductPriceValidation : IValidationStrategy<Product>
    {
        public void Validate(Product product)
        {
            if (product.Price <= 0)
            {
                throw new ArgumentException("O preço do produto deve ser maior que zero.");
            }
        }
    }
}
