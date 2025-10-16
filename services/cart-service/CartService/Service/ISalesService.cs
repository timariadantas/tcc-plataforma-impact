using CartService.Domain;
using System.Collections.Generic;

namespace CartService.Service;

    public interface ISalesService

    {
        Sale CreateSale(string clientId);
        void AddItem(string saleId, string productId, int quantity);
        void UpdateItemQuantity(string itemId, int quantity);
        void CancelSale(string saleId);
        Sale? GetSaleById(string saleId);
        List<Sale> GetSalesByProduct(string productId);
        List<Sale> GetSalesByStatus(int status);
    }
