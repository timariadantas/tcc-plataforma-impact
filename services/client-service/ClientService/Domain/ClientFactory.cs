using System;
using NUlid;
namespace ClientService.Domain
{
    public static class ClientFactory
    {
        public static Client Create(string name, string surname, string email, DateTime birthdate)
        {
            return new Client
            {
                Id = NUlid.Ulid.NewUlid().ToString(),
                Name = name,
                Surname = surname,
                Email = email,
                Birthdate = birthdate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true
            };
        }
    }
}
// Anotações IPC 
// O Factory Pattern é um padrão de criação de objetos.
// Garantir que os objetos sejam criados de forma consistente.
// Ele gera instâncias prontas para uso, evitando duplicação de código e melhorando manutenção e teste.
// Facilitar mudanças futuras na criação de objetos.