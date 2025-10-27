using System;
using Xunit;
using ClientService.Domain;
using ClientService.Service;
using ClientService.Controllers;
using ClientService.DTO.Requests;
using ClientService.DTO.Responses;
using ClientService.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.IntegrationTests
{
    public class ClientIntegrationTests
    {
        private readonly ClientController _controller;

        public ClientIntegrationTests()
        {
            // Configura storage fake ou real (aqui usamos InMemory)
            var storage = new ClientService.Tests.Factories.ClientFactoryInMemory();
            var service = new ClientService.Service.ClientService(storage);
            var logger = new LoggerService();

            _controller = new ClientController(service, logger);
        }

        [Fact]
        public void CreateClient_ShouldReturnApiResponse()
        {
            var dto = new CreateClientDto
            {
                Name = "Integration",
                Surname = "Test",
                Email = "integration@test.com",
                Birthdate = new DateTime(1990, 1, 1)
            };

            var result = _controller.Create(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ClientResponseDto>>(okResult.Value);

            Assert.Equal("Cliente criado com sucesso", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("Integration", apiResponse.Data.Name);
        }
    }
}
