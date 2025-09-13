using ClientService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

// Factory não é teste, apenas implementa IClientStorage em memória.

namespace ClientService.Tests.ClientFactorys
{
    public class ClientFactoryInMemory : IClientStorage
    {
        private readonly List<Client> _clients = new();

        public void Create(Client client)
        {
            client.Id ??= Ulid.NewUlid().ToString();
            client.CreatedAt = DateTime.UtcNow;
            _clients.Add(client);
        }

        public Client? GetById(string id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
        }

        public List<Client> GetAll()
        {
            return _clients.ToList();
        }

        public void Update(Client client)
        {
            var existing = _clients.FirstOrDefault(c => c.Id == client.Id);
            if (existing != null)
            {
                existing.Name = client.Name;
                existing.Surname = client.Surname;
                existing.Email = client.Email;
                existing.Birthdate = client.Birthdate;
                existing.UpdatedAt = client.UpdatedAt;
                existing.Active = client.Active;
            }
        }

        public void Delete(string id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null) _clients.Remove(client);
        }
    }
}
