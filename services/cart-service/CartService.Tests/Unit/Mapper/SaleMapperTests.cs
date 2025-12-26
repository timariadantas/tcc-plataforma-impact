using Xunit;
using CartService.Domain;
using CartService.Mapper;
using CartService.DTO.Responses;

namespace CartService.Tests.Unit.Mapper
{
    public class SaleMapperTests
    {
        [Fact]
        public void MapSaleToResponse_Should_Map_Status()
        {
            // Arrange
            var sale = new Sale
            {
                Id = "sale-1",
                ClientId = "client1",
                Status = (int)SaleStatus.Started
            };

            // Act
            var response = SaleMapper.ToResponse(sale);

            // Assert
            Assert.Equal(sale.Id, response.Id);
            Assert.Equal(sale.ClientId, response.ClientId);
            Assert.Equal("Started", response.Status.Name);

        }
    }
}
