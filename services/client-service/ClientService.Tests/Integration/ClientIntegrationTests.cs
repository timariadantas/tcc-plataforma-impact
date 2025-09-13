using ClientService.Domain;
using ClientService.Service;
using ClientService.Storage;
using System;
using System.Collections.Generic;
using Xunit;

namespace ClientService.Tests.Integration
{
    public class ClientIntegrationTests : IDisposable
    {
        private readonly ClientService.Service.ClientService _service;
        private readonly ClientStorage _storage;

        public ClientIntegrationTests()
        {
            // Pega a connection string do ambiente
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string não definida");

            _storage = new ClientStorage(connectionString); // storage real
            _service = new ClientService.Service.ClientService(_storage); // service real
        }

        [Fact]
        public void CreateClient_ShouldAddClientInDatabase()
        {
            var client = new Client
            {
                Name = "Docker",
                Surname = "Test",
                Email = "docker@test.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);

            var result = _service.GetById(client.Id);

            Assert.NotNull(result);
            Assert.Equal(client.Name, result!.Name);
        }

        [Fact]
        public void UpdateClient_ShouldModifyClientInDatabase()
        {
            var client = new Client
            {
                Name = "Docker",
                Surname = "Test",
                Email = "docker@test.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);

            // Atualizar
            client.Name = "Docker Updated";
            client.UpdatedAt = DateTime.UtcNow;
            _service.Update(client);

            var updated = _service.GetById(client.Id);
            Assert.Equal("Docker Updated", updated!.Name);
        }

        [Fact]
        public void DeleteClient_ShouldRemoveClientFromDatabase()
        {
            var client = new Client
            {
                Name = "Docker",
                Surname = "Test",
                Email = "docker@test.com",
                Birthdate = DateTime.UtcNow,
                Active = true
            };

            _service.Create(client);
            _service.Delete(client.Id);

            var deleted = _service.GetById(client.Id);
            Assert.Null(deleted);
        }

        public void Dispose()
        {
            // Aqui você poderia limpar dados criados no banco
        }
    }
}
