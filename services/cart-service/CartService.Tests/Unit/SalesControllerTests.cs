using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CartService.Controllers;
using CartService.Service;
using CartService.Domain;
using CartService.DTO.Requests;
using CartService.DTO.Responses;
using CartService.Tests.Factories;
using CartService.Logging; // Import necessário para LoggerService

namespace CartService.Tests.Unit
{
    public class SalesControllerTests
    {
        private readonly SaleController _controller;
        private readonly SalesService _service;
        private readonly SalesFactoryInMemory _storage;
        private readonly LoggerService _logger; // Logger adicionado

        public SalesControllerTests()
        {
            _storage = new SalesFactoryInMemory();
            _service = new SalesService(_storage);
            _logger = new LoggerService(); // Cria logger em memória para testes
            _controller = new SaleController(_service, _logger); // Passa logger para o controller
        }

        [Fact]
        public void CreateSale_ValidRequest_ReturnsOkWithSaleResponse()
        {
            var request = new SaleRequestDTO { ClientId = "123" };
            var result = _controller.CreateSale(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<SaleResponseDTO>>(okResult.Value!);

            Assert.Equal("Venda criada com sucesso", apiResponse.Message);
            Assert.Equal("123", apiResponse.Data!.ClientId);
            Assert.Equal((int)SaleStatus.Started, apiResponse.Data.Status);
            Assert.False(string.IsNullOrEmpty(apiResponse.Data.Id));
        }

        [Fact]
        public void GetSaleById_ExistingSale_ReturnsOkWithSaleResponse()
        {
            var sale = _service.CreateSale("c1");
            var result = _controller.GetSaleById(sale.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<SaleResponseDTO>>(okResult.Value!);

            Assert.Equal("Venda encontrada", apiResponse.Message);
            Assert.Equal(sale.Id, apiResponse.Data!.Id);
        }

        [Fact]
        public void GetSaleById_NonExistingSale_ReturnsNotFound()
        {
            var result = _controller.GetSaleById("invalid-id");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value!);

            Assert.Equal("Venda não encontrada", apiResponse.Message);
            Assert.Equal("NotFound", apiResponse.Error);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public void AddItem_ValidItem_ReturnsOk()
        {
            var sale = _service.CreateSale("c1");
            var itemDto = new SaleItemRequestDTO
            {
                ProductId = "p1",
                Quantity = 2
            };

            var result = _controller.AddItem(sale.Id, itemDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value!);

            Assert.Equal("Item adicionado com sucesso", apiResponse.Message);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public void UpdateItemQuantity_ValidRequest_ReturnsOk()
        {
            var sale = _service.CreateSale("c1");
            _service.AddItem(sale.Id, "p1", 1);
            var item = _service.GetSaleById(sale.Id)!.Items[0];

            var dto = new SaleItemRequestDTO { Quantity = 5 };
            var result = _controller.UpdateItemQuantity(item.Id, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value!);

            Assert.Equal("Quantidade atualizada com sucesso", apiResponse.Message);
            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public void CancelSale_ValidSale_ReturnsOkWithUpdatedSale()
        {
            var sale = _service.CreateSale("c1");
            var result = _controller.CancelSale(sale.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value!);

            Assert.Equal("Venda cancelada com sucesso", apiResponse.Message);

            // Verifica que o status foi atualizado
            var updatedSale = _service.GetSaleById(sale.Id);
            Assert.Equal((int)SaleStatus.Canceled, updatedSale!.Status);
        }
    }
}
