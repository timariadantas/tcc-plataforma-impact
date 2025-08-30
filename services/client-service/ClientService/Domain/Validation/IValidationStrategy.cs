namespace ClientService.Domain.Validation
{
    public interface IValidationStrategy<T>
    {
        void Validate(T entity);
    }
}

//Anotações IPC
//Usa Strategy Pattern: você pode adicionar outras validações (nome, sobrenome, birthdate) sem modificar o Client.


