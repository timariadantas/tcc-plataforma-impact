namespace ClientService.Domain.Exceptions;

public class InvalidBirthdateException : DomainException
{
    public InvalidBirthdateException(DateTime birthdate)
        : base($"Data de nascimento inválida: {birthdate:yyyy-MM-dd}") { }
}
