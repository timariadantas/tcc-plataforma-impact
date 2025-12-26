using CartService.DTO.Responses;
namespace CartService.DTO.Requests;

public class SaleRequestDTO
{
    public string ClientId { get; set; } = null!;
    //public SaleStatusDTO? Status { get; set; } = new();
    public List<SaleItemRequestDTO> Items { get; set; } = new List<SaleItemRequestDTO>();
}

public class SaleItemRequestDTO
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
}
