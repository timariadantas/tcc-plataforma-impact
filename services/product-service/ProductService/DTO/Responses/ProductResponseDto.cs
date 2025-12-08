namespace ProductService.DTO.Responses
{
    // DTO de saída para retornar produto
    public class ProductResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }
    }
}
