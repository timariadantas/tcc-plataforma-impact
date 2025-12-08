using System;
using System.Collections.Generic;
using CartService.Domain;
using CartService.Storage;
using CartService.Logging;

namespace CartService.Service;

public class SalesService : ISalesService
{
    //Sólido — Princípio DIP (Dependency Inversion) "Dependa de abstrações, não de implementações."
    private readonly ISalesStorage _storage;
    private readonly ILoggerService _logger;

    public SalesService(ISalesStorage storage, ILoggerService logger)
    {
        _storage = storage;
        _logger = logger;
    }

    // Criar venda
    public Sale CreateSale(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("ClientId não pode ser vazio.");

        //cria o objeto com status inicial.
        var sale = new Sale
        {
            ClientId = clientId,
            Status = (int)SaleStatus.Started,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _storage.Create(sale); // Salva no banco.
        _logger.LogInformation("Venda criada com sucesso para Cliente {ClientId}", clientId); // Faz log com template.

        return sale; // retorna a venda
    }

    // Adicionar item à venda
    public void AddItem(string saleId, string productId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(saleId))
            throw new ArgumentException("SaleId não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId não pode ser vazio.");

        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        //Busca venda:
        var sale = _storage.GetById(saleId);
        if (sale is null)
            throw new InvalidOperationException("Venda não encontrada.");

        // Verifica status:
        if (sale.Status == (int)SaleStatus.Canceled)
            throw new InvalidOperationException("Não é permitido adicionar itens em venda cancelada.");

        if (sale.Status == (int)SaleStatus.Done)
            throw new InvalidOperationException("Não é permitido adicionar itens em venda finalizada.");
        
        //Cria o item:
        var saleItem = new SaleItem
        {
            SellId = saleId,
            ProductId = productId,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _storage.AddItem(saleItem); // Persiste

         // Atualiza status se necessário
        if (sale.Status == (int)SaleStatus.Started)
            _storage.UpdateStatus(saleId, (int)SaleStatus.Progress);

        _logger.LogInformation(
            "Item adicionado à venda {SaleId} | Produto {ProductId} | Quantidade {Quantity}",
            saleId, productId, quantity
        );
    }

    // Alterar quantidade de item já existente
    public void UpdateItemQuantity(string itemId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("ItemId não pode ser vazio.");
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        // busca item e o id 
        var item = _storage.GetItemById(itemId);
        if (item is null)
            throw new InvalidOperationException("Item não encontrado.");

        var sale = _storage.GetById(item.SellId);
        if (sale is null)
            throw new InvalidOperationException("Venda não encontrada.");

         // Verifica status:
        if (sale.Status == (int)SaleStatus.Canceled)
            throw new InvalidOperationException("Venda cancelada não pode ser alterada.");
        if (sale.Status == (int)SaleStatus.Done)
            throw new InvalidOperationException("Venda finalizada não pode ser alterada.");

       

        _storage.UpdateItemQuantity(itemId, quantity); //Persiste
        _logger.LogInformation(
            "Quantidade do item {ItemId} alterada para {Quantity}",
            itemId, quantity
        );
    }

    // Cancelar venda
    public void CancelSale(string saleId)
    {
        if (string.IsNullOrWhiteSpace(saleId))
            throw new ArgumentException("SaleId não pode ser vazio.");

        var sale = _storage.GetById(saleId);
        if (sale is null)
            throw new InvalidOperationException("Venda não encontrada.");

        //Evita chamar o storage sem necessidade
        if (sale.Status == (int)SaleStatus.Canceled)
        {
            _logger.LogWarning("Cancelamento ignorado. Venda {SaleId} já estava cancelada.", saleId);
            return;
        }

        _storage.UpdateStatus(saleId, (int)SaleStatus.Canceled); // Persiste
        _logger.LogInformation("Venda {SaleId} cancelada com sucesso.", saleId);
    }

    // Buscar venda por ID
    public Sale? GetSaleById(string saleId)
    {
        if (string.IsNullOrWhiteSpace(saleId))
            throw new ArgumentException("SaleId não pode ser vazio.");

        try
        {
            var sale = _storage.GetById(saleId);

            if (sale is null)
            {
                _logger.LogWarning("Nenhuma venda encontrada para SaleId {SaleId}", saleId);
                return null;
            }

            _logger.LogInformation("Venda encontrada {SaleId}", saleId);
            return sale;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar a venda {SaleId}", saleId);
            throw;
        }
    }

    // Buscar vendas por productId
    public List<Sale> GetSalesByProduct(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ProductId não pode ser vazio.");

        try
        {
            var sales = _storage.GetSalesByProduct(productId);

            if (sales is null || sales.Count == 0)
            {
                _logger.LogInformation("Nenhuma venda encontrada para ProductId {ProductId}", productId);
                return new List<Sale>();
            }

            _logger.LogInformation(
                "{Quantidade} vendas encontradas para ProductId {ProductId}",
                sales.Count, productId);

            return sales;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar vendas do ProductId {ProductId}", productId);
            throw;
        }
    }
}


 // futuramente verificar estoque ...