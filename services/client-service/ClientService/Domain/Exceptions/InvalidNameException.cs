using System;

namespace ClientService.Domain.Exceptions;

public class InvalidNameException : Exception
{
    public InvalidNameException()
    {
    }

    public InvalidNameException(string message)
        : base(message)
    {
    }
}