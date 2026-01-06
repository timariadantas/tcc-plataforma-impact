namespace ClientService.Domain.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException()
        : base("Nome e sobrenome são obrigatórios.") { }
}
