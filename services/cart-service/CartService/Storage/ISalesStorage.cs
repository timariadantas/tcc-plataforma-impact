using CartService.Domain;
using System.Collections.Generic;

namespace CartService.Storage;

    public interface ISalesStorage
    {
     void Create(Sale sale);
        Sale? GetById(string id);
        void UpdateStatus(string saleId, int status);
        void CancelSale(string saleId);

        void AddItem(SaleItem item);
        void UpdateItemQuantity(string itemId, int quantity);

        List<Sale> GetSalesByProduct(string productId);
        int GetSalesCountByProductAndStatus(string productId, int status);
        List<Sale> GetSalesByStatus(int status);
        List<SaleItem> GetItemsBySaleId(string saleId);
    }
