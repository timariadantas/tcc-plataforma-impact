using NUlid;
using System;
using System.Collections.Generic;

namespace CartService.Domain;

    public class Sale
    {
        public string Id { get; set; } = null!;    // ULID-like
        public string ClientId { get; set; } = null!;  // Identificador do cliente
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();
        public int Status { get; set; }  // estado da venda enum
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }



    public enum SaleStatus
    {
        Started = 0,   // Venda criada
        Progress = 1,  // Primeiro item adicionado
        Done = 2,      // Finalizada
        Canceled = 3   // Cancelada
    }


