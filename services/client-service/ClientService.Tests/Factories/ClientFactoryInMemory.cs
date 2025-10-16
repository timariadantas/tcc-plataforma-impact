using System;
using System.Collections.Generic;
using System.Linq;
using ClientService.Domain;

namespace ClientService.Tests.Factories
{
    // Fake storage para testes
    public class ClientFactoryInMemory : IClientStorage
    {
        private readonly List<Client> _clients = new();

        public void Create(Client client)
        {
            if (string.IsNullOrEmpty(client.Id))
                client.Id = Guid.NewGuid().ToString();
            client.CreatedAt = DateTime.UtcNow;
            client.UpdatedAt = DateTime.UtcNow;
            _clients.Add(client);
        }

        public List<Client> GetAll()
        {
            return _clients.ToList();
        }

        public Client? GetById(string id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
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
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Delete(string id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clients.Remove(client);
        }
    }
}
