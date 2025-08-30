namespace ClientService.Domain.Validation;

public class EmailValidation : IValidationStrategy <Client>
{
    public void Validate(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Email) || !client.Email.Contains("@"))
            throw new ArgumentException("Email inválido !");
    }
}
// Anotações IPC
// Cria uma regra de validação específica para o email do cliente.
//Faz parte do domínio (camada Domain), separando regras de negócio das camadas de serviço e storage.
//Permite reaproveitar a validação em qualquer lugar que trabalhe com Client.