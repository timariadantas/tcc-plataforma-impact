using CartService.Domain;
using System.Collections.Generic;

namespace CartService.Storage;

    public interface ISalesStorage
    {
        void Create(Sale sale);
        Sale? GetById(string id);
        void UpdateStatus(string saleId, SaleStatus status);
        void CancelSale(string saleId);

        void AddItem(SaleItem item);
        void UpdateItemQuantity(string itemId, int quantity);

        List<Sale> GetSalesByProduct(string productId);
        int GetSalesCountByProductAndStatus(string productId, SaleStatus status);
        List<Sale> GetSalesByStatus(SaleStatus status);
        List<SaleItem> GetItemsBySaleId(string saleId);
        SaleItem? GetItemById(string itemId);
       

    }
