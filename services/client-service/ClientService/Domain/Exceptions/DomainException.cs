namespace ClientService.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message){ }
}

public class ClientNotFoundException: DomainException{
        public  ClientNotFoundException(string id):base($"Cliente {id} não encontrado"){}
}

public class ClientValidationException : DomainException
{
    public ClientValidationException(string message) : base(message){}
}