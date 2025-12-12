using CartService.DTO;

namespace CartService.DTO.Responses;
  public class SaleResponseDTO
    {
        public string Id { get; set; } = null!;
        public string ClientId { get; set; } = null!;
       public SaleStatusDTO Status { get; set; }= new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SaleItemResponseDTO> Items { get; set; } = new List<SaleItemResponseDTO>();
    }

     public class SaleItemResponseDTO
    {
        public string Id { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

