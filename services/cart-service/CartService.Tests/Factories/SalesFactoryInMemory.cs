using System;
using System.Collections.Generic;
using CartService.Domain;
using CartService.Storage;
using NUlid;


//A Factory em memória (SalesFactoryInMemory) já está cuidando de salvar Sale e SaleItem sem precisar do banco real.

namespace CartService.Tests.Factories
{
    public class SalesFactoryInMemory : ISalesStorage
    {
        private readonly List<Sale> _sales = new();
        private readonly List<SaleItem> _items = new();

        public void Create(Sale sale)
        {
            sale.Id = Ulid.NewUlid().ToString();
            sale.CreatedAt = DateTime.UtcNow;
            sale.UpdatedAt = DateTime.UtcNow;
            _sales.Add(sale);
        }

        public Sale? GetById(string id)
        {
            var sale = _sales.FirstOrDefault(s => s.Id == id);
            if (sale != null)
                sale.Items = GetItemsBySaleId(id);
            return sale;
        }

        public void UpdateStatus(string saleId, int status)
        {
            var sale = _sales.FirstOrDefault(s => s.Id == saleId);
            if (sale != null)
            {
                sale.Status = status;
                sale.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void CancelSale(string saleId)
        {
            UpdateStatus(saleId, (int)SaleStatus.Canceled);
        }

        public void AddItem(SaleItem item)
        {
            item.Id = item.Id ?? Guid.NewGuid().ToString();
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            _items.Add(item);
        }

        public void UpdateItemQuantity(string itemId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }

        public List<SaleItem> GetItemsBySaleId(string saleId)
        {
            return _items.Where(i => i.SellId == saleId).ToList();
        }

        public List<Sale> GetSalesByProduct(string productId)
        {
            var saleIds = _items.Where(i => i.ProductId == productId).Select(i => i.SellId).Distinct();
            return _sales.Where(s => saleIds.Contains(s.Id)).ToList();
        }

        public int GetSalesCountByProductAndStatus(string productId, int status)
        {
            var saleIds = _items.Where(i => i.ProductId == productId).Select(i => i.SellId).Distinct();
            return _sales.Count(s => saleIds.Contains(s.Id) && s.Status == status);
        }

        public List<Sale> GetSalesByStatus(int status)
        {
            return _sales.Where(s => s.Status == status).ToList();
        }

        public SaleItem? GetItemById(string itemId)
        {
            foreach (var item in _items)
            {
                if (item.Id == itemId)
                {
                    return item;
                }
            }

            // Se não encontrou nenhum item com esse Id
            return null;
        }

    }
}


