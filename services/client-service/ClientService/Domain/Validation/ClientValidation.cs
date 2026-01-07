using ClientService.Domain.Exceptions;

namespace ClientService.Domain.Validation;


public static class ClientValidation
{
    private const int MaxNameLength = 100;
    private const int MaxSurnameLength = 200;
    private const int MaxEmailLength = 320;

    public static void Validate(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.Surname))
            throw new InvalidNameException();

        if (client.Name.Length > MaxNameLength)
            throw new InvalidNameException($"Nome não pode ter mais de {MaxNameLength} caracteres");
        
        if (client.Surname.Length > MaxSurnameLength)
            throw new InvalidNameException($"Sobrenome não pode ter mais que {MaxSurnameLength} caracteres");
        
        if (string.IsNullOrWhiteSpace(client.Email) || !client.Email.Contains("@"))
            throw new InvalidEmailException(client.Email);
        
        if (client.Email.Length > MaxEmailLength)
            throw new InvalidEmailException("Email muito longo");


        if (client.Birthdate > DateTime.UtcNow)
            throw new InvalidBirthdateException(client.Birthdate);
            var age = DateTime.UtcNow.Year - client.Birthdate.Year;
            if (age < 0 || age > 120)
                throw new InvalidBirthdateException("Idade Inválida");
    }
}
