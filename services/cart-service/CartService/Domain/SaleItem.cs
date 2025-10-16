using NUlid;
using System;



namespace CartService.Domain;

    public class SaleItem
    {
        public string Id { get; set; } = null!;                 // ULID-like
        public string SellId { get; set; } = null!;            // Referência para a venda
        public string ProductId { get; set; } = null!;         // Produto
        public int Quantity { get; set; }                      
        public DateTime CreatedAt { get; set; }                
        public DateTime UpdatedAt { get; set; }                

    }







