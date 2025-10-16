using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ClientService.Domain;
using ClientService.Service;
using ClientService.Tests.Factories;
using ClientService.Controllers;
using ClientService.DTO.Requests;  // DTOs de request
using ClientService.DTO.Responses; // DTOs de resposta
using ClientService.Logging;

namespace ClientService.Tests.Unit
{
    public class ClientServiceTests
    {
        private readonly ClientController _controller;
        private readonly ClientFactoryInMemory _storage;
        private readonly ClientService.Service.ClientService _service;

        public ClientServiceTests()
        {
            _storage = new ClientFactoryInMemory();
            _service = new ClientService.Service.ClientService(_storage);
            var logger = new LoggerService(); // fake logger
            _controller = new ClientController(_service, logger);
        }

        [Fact]
        public void CreateClient_WithValidDto_ShouldReturnOk()
        {
            // Arrange
            var dto = new CreateClientDto
            {
                Name = "Maria",
                Surname = "Dantas",
                Email = "maria@test.com",
                Birthdate = DateTime.Parse("2000-01-01")
            };

            // Act
            var result = _controller.Create(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdClient = Assert.IsType<ClientResponseDto>(okResult.Value);
            Assert.Equal("Maria", createdClient.Name);
            Assert.Equal("Dantas", createdClient.Surname);
            Assert.False(string.IsNullOrEmpty(createdClient.Id)); // ULID gerado
        }

        [Fact]
        public void CreateClient_WithEmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var dto = new CreateClientDto
            {
                Name = "",
                Surname = "Dantas",
                Email = "maria@test.com",
                Birthdate = DateTime.Parse("2000-01-01")
            };

            // Act
            var result = _controller.Create(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("Nome é obrigatório", error.Message);
        }

        [Fact]
        public void GetById_ShouldReturnClientResponseDto()
        {
            // Arrange
            var client = ClientFactory.Create("Carlos", "Silva", "carlos@test.com", DateTime.Parse("1985-05-05"));
            _service.Create(client);

            // Act
            var result = _controller.GetById(client.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedClient = Assert.IsType<ClientResponseDto>(okResult.Value);
            Assert.Equal("Carlos", returnedClient.Name);
        }

        [Fact]
        public void GetAll_ShouldReturnAllClientsResponseDto()
        {
            // Arrange
            var c1 = ClientFactory.Create("A", "B", "a@test.com", DateTime.Parse("2000-01-01"));
            var c2 = ClientFactory.Create("C", "D", "c@test.com", DateTime.Parse("1995-01-01"));
            _service.Create(c1);
            _service.Create(c2);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<ClientResponseDto>>(okResult.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void UpdateClient_ShouldChangeClientData()
        {
            // Arrange
            var client = ClientFactory.Create("Old", "Name", "old@test.com", DateTime.Parse("1990-01-01"));
            _service.Create(client);

            var updateDto = new UpdateClientDto
            {
                Name = "New",
                Surname = "Name",
                Email = "new@test.com",
                Birthdate = client.Birthdate,
                Active = true
            };

            // Act
            var result = _controller.Update(client.Id, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<ClientResponseDto>(okResult.Value);
            Assert.Equal("New", updated.Name);
            Assert.Equal("new@test.com", updated.Email);

            // Verifica no storage
            var stored = _storage.GetById(client.Id);
            Assert.Equal("New", stored.Name);
        }

        [Fact]
        public void DeleteClientShouldReturnSuccessMessage()
        {
            // Arrange
            var client = ClientFactory.Create("ToDelete", "User", "delete@test.com", DateTime.Parse("1990-01-01"));
            _service.Create(client);

            // Act
            var result = _controller.Delete(client.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<DeleteResponseDto>(okResult.Value);
            Assert.Equal($"Cliente deletado com sucesso: ID {client.Id}", response.Message);

            // Verifica remoção
            Assert.Null(_storage.GetById(client.Id));
        }

        [Fact]
        public void DeleteClient_NonExistent_ShouldReturnBadRequest()
        {
            // Arrange
            string nonExistentId = "999";

            // Act
            var result = _controller.Delete(nonExistentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Cliente não encontrado", response.Message);
        }
    }
}
