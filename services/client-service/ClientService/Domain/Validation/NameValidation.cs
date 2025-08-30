namespace ClientService.Domain.Validation;

public class NameValidation : IValidationStrategy<Client>
{
    public void Validate(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new ArgumentException("Nome é obrigatório");
        if (string.IsNullOrWhiteSpace(client.Surname))
            throw new ArgumentException("Sobrenome é obrigátio.");
    }
}

//Anotações IPC
//A classe valida que o nome e o sobrenome do cliente não podem estar vazios.
//Faz parte da camada de domínio, mantendo as regras de negócio separadas do serviço e do storage.
//Segue o Strategy Pattern, permitindo que você adicione outras validações (EmailValidation, BirthdateValidation, etc.) sem modificar a entidade Client.