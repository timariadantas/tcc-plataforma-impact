using System;
using System.Collections.Generic;
using System.Linq;
using CartService.Domain;
using CartService.Storage;


namespace CartService.Service;

      public class SalesService : ISalesService
    {
        private readonly ISalesStorage _storage;

        public SalesService(ISalesStorage storage)
        {
            _storage = storage;
        }

        // Criar venda
        public Sale CreateSale(string clientId)
        {
            var sale = new Sale
            {
                ClientId = clientId,
                Status = (int)SaleStatus.Started,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _storage.Create(sale);
            return sale;
        }

        // Adicionar item à venda
        public void AddItem(string saleId, string productId, int quantity)
        {
            var saleItem = new SaleItem
            {
                SellId = saleId,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _storage.AddItem(saleItem);

            // Atualiza status para Progress se estava Started
            var sale = _storage.GetById(saleId);
            if (sale != null && sale.Status == (int)SaleStatus.Started)
            {
                _storage.UpdateStatus(saleId, (int)SaleStatus.Progress);
            }
        }

        // Alterar quantidade de item
        public void UpdateItemQuantity(string itemId, int quantity)
        {
            _storage.UpdateItemQuantity(itemId, quantity);
        }

        // Cancelar venda
        public void CancelSale(string saleId)
        {
            _storage.CancelSale(saleId);
        }

        // Buscar venda
        public Sale? GetSaleById(string saleId)
        {
            return _storage.GetById(saleId);
        }

        // Consultar vendas por produto
        public List<Sale> GetSalesByProduct(string productId)
        {
            return _storage.GetSalesByProduct(productId);
        }

        // Consultar vendas por status
        public List<Sale> GetSalesByStatus(int status)
        {
            return _storage.GetSalesByStatus(status);
        }
    }
