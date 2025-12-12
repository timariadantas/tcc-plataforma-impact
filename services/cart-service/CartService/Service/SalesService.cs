using System;
using System.Collections.Generic;
using CartService.Domain;
using CartService.Storage;
using CartService.Logging;
using CartService.Domain.Exceptions;
using NUlid;
namespace CartService.Service
{
    public class SalesService : ISalesService
    {
        private readonly ISalesStorage _storage;
        private readonly ILoggerService _logger;

        public SalesService(ISalesStorage storage, ILoggerService logger)
        {
            _storage = storage;
            _logger = logger;
        }

       // criar venda
        public Sale CreateSale(string clientId, List<SaleItem>? items = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new DomainValidationException("ClientId não pode ser vazio.");

            var sale = new Sale
            {
                Id = Ulid.NewUlid().ToString(),          
                ClientId = clientId,
                Status = (int)SaleStatus.Started,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Cria venda sem itens inicialmente
            _storage.Create(sale);

            // Sem itens → finaliza
            if (items == null || items.Count == 0)
            {
                _logger.LogInformation("Venda criada (sem itens) para Cliente {ClientId}", clientId);
                return sale;
            }

            // Se veio itens → adiciona
            foreach (var item in items)
            {
                item.Id = Ulid.NewUlid().ToString();     // <-- ITEM TAMBÉM PRECISA ID AGORA
                item.SellId = sale.Id;
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;

                _storage.AddItem(item);
            }

            // Atualiza status
            _storage.UpdateStatus(sale.Id, (int)SaleStatus.Progress);

            _logger.LogInformation("Venda criada (com itens) para Cliente {ClientId}", clientId);

            return sale;
        }

        
        // Adicionar item à venda
        public void AddItem(string saleId, string productId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(saleId))
                throw new DomainValidationException("SaleId não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(productId))
                throw new DomainValidationException("ProductId não pode ser vazio.");

            if (quantity <= 0)
                throw new DomainValidationException("Quantidade deve ser maior que zero.");

            var sale = _storage.GetById(saleId);
            if (sale is null)
                throw new NotFoundException("Venda não encontrada.");

            if (sale.Status == (int)SaleStatus.Canceled)
                throw new BusinessRuleException("Não é permitido adicionar itens em venda cancelada.");

            if (sale.Status == (int)SaleStatus.Done)
                throw new BusinessRuleException("Não é permitido adicionar itens em venda finalizada.");

            var saleItem = new SaleItem
            {
                Id = Ulid.NewUlid().ToString(), 
                SellId = saleId,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _storage.AddItem(saleItem);

            if (sale.Status == (int)SaleStatus.Started)
                _storage.UpdateStatus(saleId, (int)SaleStatus.Progress);

            _logger.LogInformation(
                "Item adicionado | SaleId: {SaleId} | Produto: {ProductId} | Qtd: {Quantity}",
                saleId, productId, quantity
            );
        }

        // Alterar quantidade de item
        public void UpdateItemQuantity(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new DomainValidationException("ItemId não pode ser vazio.");

            if (quantity <= 0)
                throw new DomainValidationException("A quantidade deve ser maior que zero.");

            var item = _storage.GetItemById(itemId);
            if (item is null)
                throw new NotFoundException("Item não encontrado.");

            var sale = _storage.GetById(item.SellId);
            if (sale is null)
                throw new NotFoundException("Venda não encontrada.");

            if (sale.Status == (int)SaleStatus.Canceled)
                throw new BusinessRuleException("Venda cancelada não pode ser alterada.");

            if (sale.Status == (int)SaleStatus.Done)
                throw new BusinessRuleException("Venda finalizada não pode ser alterada.");

            _storage.UpdateItemQuantity(itemId, quantity);

            _logger.LogInformation(
                "Quantidade do item {ItemId} alterada para {Quantity}",
                itemId, quantity
            );
        }

        // Cancelar venda
        public void CancelSale(string saleId)
        {
            if (string.IsNullOrWhiteSpace(saleId))
                throw new DomainValidationException("SaleId não pode ser vazio.");

            var sale = _storage.GetById(saleId);
            if (sale is null)
                throw new NotFoundException("Venda não encontrada.");

            if (sale.Status == (int)SaleStatus.Canceled)
            {
                _logger.LogWarning("Cancelamento ignorado. Venda {SaleId} já estava cancelada.", saleId);
                return;
            }

            _storage.UpdateStatus(saleId, (int)SaleStatus.Canceled);

            _logger.LogInformation("Venda {SaleId} cancelada com sucesso.", saleId);
        }

        // Buscar venda por ID
        public Sale GetSaleById(string saleId)
        {
            if (string.IsNullOrWhiteSpace(saleId))
                throw new DomainValidationException("SaleId não pode ser vazio.");

            var sale = _storage.GetById(saleId);

            if (sale is null)
                throw new NotFoundException("Venda não encontrada.");

            _logger.LogInformation("Venda encontrada {SaleId}", saleId);

            return sale;
        }

        // Buscar vendas por productId
        public List<Sale> GetSalesByProduct(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                throw new DomainValidationException("ProductId não pode ser vazio.");

            var sale = _storage.GetSalesByProduct(productId);

            if (sale is null || sale.Count == 0)
                throw new NotFoundException("Nenhuma venda encontrada para este produto.");

            _logger.LogInformation(
                "{Quantidade} vendas encontradas para ProductId {ProductId}",
                sale.Count, productId
            );

            return sale;
        }
        // Buscar vendas por Status
public List<Sale> GetSalesByStatus(int status)
{
    var sale = _storage.GetSalesByStatus(status);

    if (sale is null || sale.Count == 0)
        throw new NotFoundException($"Nenhuma venda encontrada com status {status}.");

    _logger.LogInformation(
        "{Quantidade} vendas encontradas com Status {Status}",
        sale.Count, status
    );

    return sale;
}
public SaleItem GetItemById(string itemId)
{
    if (string.IsNullOrWhiteSpace(itemId))
        throw new DomainValidationException("ItemId não pode ser vazio.");

    var item = _storage.GetItemById(itemId);

    if (item is null)
        throw new NotFoundException("Item não encontrado.");

    _logger.LogInformation("Item encontrado {ItemId}", itemId);

    return item;
}

    }
}
