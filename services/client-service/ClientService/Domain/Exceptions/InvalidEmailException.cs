namespace ClientService.Domain.Exceptions;
public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string email): base($"Email inválido: {email}") { }
}
