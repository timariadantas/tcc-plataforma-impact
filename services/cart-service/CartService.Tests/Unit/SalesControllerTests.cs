using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CartService.Controllers;
using CartService.Service;
using CartService.Domain;
using CartService.DTO.Requests;
using CartService.DTO.Responses;
using CartService.Tests.Factories;
using CartService.Logging;


namespace CartService.Tests.Unit
{
    public class SalesControllerTests
    {
        private readonly SaleController _controller;
        private readonly SalesService _service;
    


        public SalesControllerTests()
        {
            var storage = new SalesFactoryInMemory();
            var logger = new LoggerFake();

            _service = new SalesService(storage, logger);
            _controller = new SaleController(_service, logger);
        }

        [Fact]
        public void CreateSale_ValidRequest_ReturnsOkWithSaleResponse()
        {
            var request = new SaleRequestDTO { ClientId = "123" };
            var result = _controller.CreateSale(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<SaleResponseDTO>>(okResult.Value);

            Assert.Equal("Venda criada com sucesso", apiResponse.Message);
            Assert.Equal(string.Empty, apiResponse.Error);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("123", apiResponse.Data!.ClientId);
            Assert.Equal("Started", apiResponse.Data!.Status.Name);
        }

        [Fact]
        public void GetSaleById_ExistingSale_ReturnsOkWithSaleResponse()
        {
            var sale = _service.CreateSale("c1");
            var result = _controller.GetSaleById(sale.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<SaleResponseDTO>>(okResult.Value);

            Assert.Equal("Venda encontrada com sucesso", apiResponse.Message);
            Assert.Equal(sale.Id, apiResponse.Data!.Id);
        }

        [Fact]
        public void GetSaleById_NonExistingSale_ReturnsNotFound()
        {
            var result = _controller.GetSaleById("invalid-id");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<SaleResponseDTO>>(notFoundResult.Value!);

            Assert.Equal("Venda não encontrada.", apiResponse.Message);
            Assert.Equal("NotFound", apiResponse.Error);
        }

        [Fact]
        public void AddItem_ValidItem_ReturnsOk()
        {
            var sale = _service.CreateSale("c1");
            var itemDto = new SaleItemRequestDTO { ProductId = "p1", Quantity = 2 };

            var result = _controller.AddItem(sale.Id, itemDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value!);

            Assert.Equal("Item adicionado com sucesso", apiResponse.Message);
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
        }

        [Fact]
        public void CancelSale_ValidSale_ReturnsOkWithUpdatedSale()
        {
            var sale = _service.CreateSale("c1");
            var result = _controller.CancelSale(sale.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value!);

            Assert.Equal("Venda cancelada com sucesso", apiResponse.Message);
        }
    }
}
