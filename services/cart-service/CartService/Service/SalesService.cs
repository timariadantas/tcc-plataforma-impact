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
        Status = SaleStatus.Started,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Items = new List<SaleItem>()
    };

    try
    {
        // Cria venda sem itens inicialmente
        _storage.Create(sale);
        _logger.LogInformation(
            $"Insert Sale -> id:{sale.Id}, client_id:{sale.ClientId}, status:{(int)sale.Status}, created_at:{sale.CreatedAt:o}, updated_at:{sale.UpdatedAt:o}"
        );

        // Sem itens → finaliza
        if (items == null || items.Count == 0)
        {
            _logger.LogInformation("Venda criada (sem itens) para Cliente {ClientId}", clientId);
            return sale;
        }

        // Se veio itens → adiciona
        foreach (var item in items)
        {
            item.Id = Ulid.NewUlid().ToString();
            item.SellId = sale.Id;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            _storage.AddItem(item);
            _logger.LogInformation(
                $"Insert SaleItem -> id:{item.Id}, sell_id:{item.SellId}, product_id:{item.ProductId}, quantity:{item.Quantity}, created_at:{item.CreatedAt:o}, updated_at:{item.UpdatedAt:o}"
            );

            sale.Items.Add(item); // Mantém consistência em memória
        }

        // Atualiza status
        _storage.UpdateStatus(sale.Id, SaleStatus.Progress);
        _logger.LogInformation($"UpdateStatus -> id:{sale.Id}, status:{(int)SaleStatus.Progress}");
    }
    catch (Exception ex)
    {
        _logger.LogError($"[CreateSale][500] Erro inesperado criando venda for client {clientId} | EXCEPTION: {ex.Message} | STACK: {ex.StackTrace}");
        throw new Exception("Erro ao criar venda", ex);
    }

    _logger.LogInformation($"Venda criada (com itens) para Cliente {clientId}");
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

            if (sale.Status == SaleStatus.Canceled)
                throw new BusinessRuleException("Não é permitido adicionar itens em venda cancelada.");

            if (sale.Status == SaleStatus.Done)
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
            sale.UpdatedAt = DateTime.UtcNow;



            sale.Items.Add(saleItem);
            if (sale.Status == SaleStatus.Started)
                _storage.UpdateStatus(saleId, SaleStatus.Progress);

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

            if (sale.Status == SaleStatus.Canceled)
                throw new BusinessRuleException("Venda cancelada não pode ser alterada.");

            if (sale.Status == SaleStatus.Done)
                throw new BusinessRuleException("Venda finalizada não pode ser alterada.");

            _storage.UpdateItemQuantity(itemId, quantity);

            // mantém objeto sincronizado em memória
            item.Quantity = quantity;
            item.UpdatedAt = DateTime.UtcNow;

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

            if (sale.Status == SaleStatus.Canceled)
            {
                _logger.LogWarning("Cancelamento ignorado. Venda {SaleId} já estava cancelada.", saleId);
                return;
            }

            _storage.UpdateStatus(saleId, SaleStatus.Canceled);

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


        public List<Sale> GetSalesByStatus(SaleStatus status)
        {
            var sales = _storage.GetSalesByStatus(status);

            if (sales == null || sales.Count == 0)
                throw new NotFoundException("Nenhuma venda encontrada com esse status.");

            _logger.LogInformation("{Count} vendas encontradas com status {Status}", sales.Count, status);

            return sales;
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
