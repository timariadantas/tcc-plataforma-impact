using System;

namespace ClientService.Domain.Exceptions;

public class InvalidBirthdateException : Exception
{
    public InvalidBirthdateException(DateTime birthdate)
        : base($"Invalid birthdate: {birthdate}")
    {
    }

    public InvalidBirthdateException(string message)
        : base(message)
    {
    }
}