using CartService.Domain;
using CartService.DTO.Responses;


namespace CartService.Mapper
{
    //O mapper pega a entidade interna (Sale) e transforma no DTO (SaleResponseDTO)
    public static class SaleMapper
{
    public static SaleResponseDTO ToResponse(Sale sale)
    {
        return new SaleResponseDTO
        {
            Id = sale.Id,
            ClientId = sale.ClientId,

            Status = new SaleStatusDTO
            {
                Name = ((SaleStatus)sale.Status).ToString()
            },

            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,

            Items = sale.Items.Select(i => new SaleItemResponseDTO
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).ToList()
        };
    }
}
}



