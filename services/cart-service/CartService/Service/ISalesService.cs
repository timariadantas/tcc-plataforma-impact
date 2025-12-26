using CartService.Domain;
using System.Collections.Generic;

namespace CartService.Service;

    public interface ISalesService

    {
        Sale CreateSale(string clientId, List<SaleItem>? items = null);
        void AddItem(string saleId, string productId, int quantity);
        void UpdateItemQuantity(string itemId, int quantity);
        void CancelSale(string saleId);
        Sale? GetSaleById(string saleId);
        List<Sale> GetSalesByProduct(string productId);
        List<Sale> GetSalesByStatus(SaleStatus status);
        SaleItem? GetItemById(string itemId);
    }
