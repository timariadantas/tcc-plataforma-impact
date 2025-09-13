using ClientService.Domain;
using ClientService.Service;
using ClientService.Tests.Domain;
using Xunit;
using System;

// Unit tests → rápido, sem Docker, usando ClientFactoryInMemory.

namespace ClientService.Tests.ClientFactorys
{
    public class ClientServiceTests
    {
        private readonly ClientService.Service.ClientService _service;

        public ClientServiceTests()
        {
            var factory = new ClientFactoryInMemory();       // storage em memória
            _service = new ClientService.Service.ClientService(factory); // Service síncrono
        }

        [Fact]
        public void CreateClient_ShouldAddClient()
        {
            var client = new Client
            {
                Name = "Maria",
                Surname = "Dantas",
                Email = "maria@email.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);

            var result = _service.GetById(client.Id);

            Assert.NotNull(result);
            Assert.Equal(client.Name, result!.Name);
        }

        [Fact]
        public void UpdateClient_ShouldModifyClient()
        {
            var client = new Client
            {
                Name = "Maria",
                Surname = "Dantas",
                Email = "maria@email.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);

            client.Name = "Maria Updated";
            client.UpdatedAt = DateTime.UtcNow;
            _service.Update(client);

            var updated = _service.GetById(client.Id);
            Assert.Equal("Maria Updated", updated!.Name);
        }

        [Fact]
        public void DeleteClient_ShouldRemoveClient()
        {
            var client = new Client
            {
                Name = "Maria",
                Surname = "Dantas",
                Email = "maria@email.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);
            _service.Delete(client.Id);

            var deleted = _service.GetById(client.Id);
            Assert.Null(deleted);
        }
    }
}
