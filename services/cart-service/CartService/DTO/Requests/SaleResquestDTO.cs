namespace CartService.DTO.Requests;

public class SaleRequestDTO
{
    public string ClientId { get; set; } = null!;
    public List<SaleItemRequestDTO>? Items { get; set; }
}

public class SaleItemRequestDTO
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
}
