using Xunit;
using System;
using System.Collections.Generic;
using CartService.Domain;
using CartService.Service;
using CartService.Tests.Factories;

namespace CartService.Tests.Unit
{
    public class SalesServiceTests
    {
        private readonly SalesFactoryInMemory _storage;
        private readonly SalesService _service;

        public SalesServiceTests()
        {
            _storage = new SalesFactoryInMemory();
            _service = new SalesService(_storage);
        }

        [Fact]
        public void CreateSale_Should_CreateSaleWithStartedStatus()
        {
            // Arrange
            string clientId = "client123";

            // Act
            var sale = _service.CreateSale(clientId);

            // Assert
            Assert.Equal(clientId, sale.ClientId);
            Assert.Equal((int)SaleStatus.Started, sale.Status);
            Assert.NotNull(sale.Id);
            Assert.NotEqual(default, sale.CreatedAt);
        }

        [Fact]
        public void AddItem_Should_AddItemAndUpdateStatusIfStarted()
        {
            // Arrange
            var sale = _service.CreateSale("client123");
            string productId = "product123";
            int quantity = 2;

            // Act
            _service.AddItem(sale.Id, productId, quantity);
            var updatedSale = _service.GetSaleById(sale.Id);

            // Assert
            Assert.Single(updatedSale!.Items);
            Assert.Equal(quantity, updatedSale.Items[0].Quantity);
            Assert.Equal((int)SaleStatus.Progress, updatedSale.Status);
        }

        [Fact]
        public void UpdateItemQuantity_Should_ChangeQuantity()
        {
            // Arrange
            var sale = _service.CreateSale("client123");
            _service.AddItem(sale.Id, "product1", 1);
            var itemId = sale.Items[0].Id;

            // Act
            _service.UpdateItemQuantity(itemId, 5);
            var updatedSale = _service.GetSaleById(sale.Id);

            // Assert
            Assert.Equal(5, updatedSale!.Items[0].Quantity);
        }

        [Fact]
        public void CancelSale_Should_SetStatusCanceled()
        {
            // Arrange
            var sale = _service.CreateSale("client123");

            // Act
            _service.CancelSale(sale.Id);
            var updatedSale = _service.GetSaleById(sale.Id);

            // Assert
            Assert.Equal((int)SaleStatus.Canceled, updatedSale!.Status);
        }

        [Fact]
        public void GetSalesByProduct_Should_ReturnSalesContainingProduct()
        {
            // Arrange
            var sale1 = _service.CreateSale("c1");
            var sale2 = _service.CreateSale("c2");

            _service.AddItem(sale1.Id, "p1", 1);
            _service.AddItem(sale2.Id, "p1", 2);
            _service.AddItem(sale2.Id, "p2", 1);

            // Act
            var salesWithP1 = _service.GetSalesByProduct("p1");

            // Assert
            Assert.Equal(2, salesWithP1.Count);
        }

        [Fact]
        public void GetSalesByStatus_Should_ReturnSalesWithStatus()
        {
            // Arrange
            var sale1 = _service.CreateSale("c1"); // Started
            var sale2 = _service.CreateSale("c2"); // Started

            _service.AddItem(sale1.Id, "p1", 1); // Progress
            _service.AddItem(sale2.Id, "p2", 1); // Progress

            // Act
            var progressSales = _service.GetSalesByStatus((int)SaleStatus.Progress);

            // Assert
            Assert.Equal(2, progressSales.Count);
        }
    }
}

// O SalesServiceTests não precisa de DTOs, porque o service trabalha com entidades (Sale e SaleItem) diretamente.
