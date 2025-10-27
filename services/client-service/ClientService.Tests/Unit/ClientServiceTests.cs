using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ClientService.Domain;
using ClientService.Service;
using ClientService.Tests.Factories;
using ClientService.Controllers;
using ClientService.DTO.Requests;
using ClientService.DTO.Responses;
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
            var logger = new LoggerService(); // Fake logger
            _controller = new ClientController(_service, logger);
        }

        [Fact]
        public void CreateClient_WithValidDto_ShouldReturnOk()
        {
            var dto = new CreateClientDto
            {
                Name = "Maria",
                Surname = "Dantas",
                Email = "maria@test.com",
                Birthdate = DateTime.Parse("2000-01-01")
            };

            var result = _controller.Create(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ClientResponseDto>>(okResult.Value);

            Assert.Equal("Cliente criado com sucesso", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("Maria", apiResponse.Data.Name);
            Assert.Equal("Dantas", apiResponse.Data.Surname);
            Assert.False(string.IsNullOrEmpty(apiResponse.Data.Id));
        }

        [Fact]
        public void CreateClient_WithEmptyName_ShouldReturnBadRequest()
        {
            var dto = new CreateClientDto
            {
                Name = "",
                Surname = "Dantas",
                Email = "maria@test.com",
                Birthdate = DateTime.Parse("2000-01-01")
            };

            var result = _controller.Create(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ClientResponseDto>>(badRequest.Value);

            Assert.Equal("Erro ao criar cliente", apiResponse.Message);
            Assert.Equal("Nome é obrigatório", apiResponse.Error);
        }

        [Fact]
        public void GetById_ShouldReturnClientResponseDto()
        {
            var client = ClientFactory.Create("Carlos", "Silva", "carlos@test.com", DateTime.Parse("1985-05-05"));
            _service.Create(client);

            var result = _controller.GetById(client.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ClientResponseDto>>(okResult.Value);

            Assert.Equal("Cliente consultado com sucesso", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("Carlos", apiResponse.Data.Name);
        }

        [Fact]
        public void GetAll_ShouldReturnAllClientsResponseDto()
        {
            var c1 = ClientFactory.Create("A", "B", "a@test.com", DateTime.Parse("2000-01-01"));
            var c2 = ClientFactory.Create("C", "D", "c@test.com", DateTime.Parse("1995-01-01"));
            _service.Create(c1);
            _service.Create(c2);

            var result = _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<ClientResponseDto>>>(okResult.Value);

            Assert.Equal("Lista de clientes recuperada com sucesso", apiResponse.Message);
            Assert.Equal(2, apiResponse.Data.Count);
        }

        [Fact]
        public void UpdateClient_ShouldChangeClientData()
        {
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

            var result = _controller.Update(client.Id, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ClientResponseDto>>(okResult.Value);

            Assert.Equal("Cliente atualizado com sucesso", apiResponse.Message);
            Assert.Equal("New", apiResponse.Data.Name);
            Assert.Equal("new@test.com", apiResponse.Data.Email);

            var stored = _storage.GetById(client.Id);
            Assert.Equal("New", stored.Name);
        }

        [Fact]
        public void DeleteClientShouldReturnSuccessMessage()
        {
            var client = ClientFactory.Create("ToDelete", "User", "delete@test.com", DateTime.Parse("1990-01-01"));
            _service.Create(client);

            var result = _controller.Delete(client.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DeleteResponseDto>>(okResult.Value);

            Assert.Equal("Cliente deletado com sucesso", apiResponse.Message);
            Assert.Equal($"Cliente deletado com sucesso: ID {client.Id}", apiResponse.Data.Message);

            Assert.Null(_storage.GetById(client.Id));
        }

        [Fact]
        public void DeleteClient_NonExistent_ShouldReturnBadRequest()
        {
            string nonExistentId = "999";

            var result = _controller.Delete(nonExistentId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<DeleteResponseDto>>(badRequestResult.Value);

            Assert.Equal("Erro ao deletar cliente", apiResponse.Message);
            Assert.Equal("Cliente não encontrado", apiResponse.Error);
        }
    }
}
