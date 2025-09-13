using ProductService.Domain;

namespace ProductService.Domain.Validation
{
    public class ProductNameValidation : IValidationStrategy<Product>
    {
        public void Validate(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("O nome do produto é obrigatório.");
            }
            if (product.Name.Length < 3)
            {
                throw new ArgumentException("O nome do produto deve ter ao menos 3 caracteres.");
            }
        }
    }
}
