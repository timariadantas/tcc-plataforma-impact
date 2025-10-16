using System;
using NUlid;

namespace ProductService.Domain
{
    public class Product
    {

        public string Id { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt{ get; set; } = DateTime.UtcNow;
        public bool Active { get; set; } = true;
    }

}

