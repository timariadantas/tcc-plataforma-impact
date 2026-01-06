using ClientService.Domain.Exceptions;

namespace ClientService.Domain.Validation;


public static class ClientValidation
{
    public static void Validate(Client client)
    {
 if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Surname))
        throw new InvalidNameException();

    if (string.IsNullOrWhiteSpace(client.Email) || !client.Email.Contains("@"))
        throw new InvalidEmailException(client.Email);

    if (client.Birthdate > DateTime.UtcNow)
        throw new InvalidBirthdateException(client.Birthdate);
    }
}
