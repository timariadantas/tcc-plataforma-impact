namespace ProductService.Domain.Validation
{
    public interface IValidationStrategy<T>
    {
        void Validate(T entity);
    }
}
// Essa interface é genérica.
//Cada validador vai implementar Validate para regras específicas.