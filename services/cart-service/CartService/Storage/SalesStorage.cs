using System;
using System.Collections.Generic;
using Npgsql;
using CartService.Domain;
using CartService.Logging;

namespace CartService.Storage;

public class SalesStorage : ISalesStorage
{
    private readonly string _connectionString;
    private readonly ILoggerService _logger;

    public SalesStorage(string connectionString, ILoggerService logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public void Create(Sale sale)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO sales(id, client_id, status, created_at, updated_at)
                VALUES (@id, @client_id, @status, @created_at, @updated_at);
            ", conn);
            cmd.Transaction = transaction;

            cmd.Parameters.AddWithValue("@id", sale.Id);
            cmd.Parameters.AddWithValue("@client_id", sale.ClientId);
            cmd.Parameters.AddWithValue("@status", (int)sale.Status);
            cmd.Parameters.AddWithValue("@created_at", sale.CreatedAt);
            cmd.Parameters.AddWithValue("@updated_at", sale.UpdatedAt);

            _logger.LogInformation(
                "Insert Sale -> id:{0}, client_id:{1}, status:{2}, created_at:{3}, updated_at:{4}",
                sale.Id, sale.ClientId, (int)sale.Status, sale.CreatedAt, sale.UpdatedAt
            );

            cmd.ExecuteNonQuery();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError("Erro ao inserir Sale: {0}", ex);
            throw;
        }
    }

    public Sale? GetById(string id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT id, client_id, status, created_at, updated_at
            FROM sales
            WHERE id = @id;", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            try
            {
                var statusInt = reader.GetInt32(reader.GetOrdinal("status"));
                var sale = new Sale
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    ClientId = reader.GetString(reader.GetOrdinal("client_id")),
                    Status = (SaleStatus)statusInt,
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    Items = GetItemsBySaleId(id)
                };

                _logger.LogInformation(
                    "GetById -> id:{0}, client_id:{1}, status:{2}, created_at:{3}, updated_at:{4}",
                    sale.Id, sale.ClientId, statusInt, sale.CreatedAt, sale.UpdatedAt
                );

                return sale;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao ler Sale do DB. id:{0}, Exception:{1}", id, ex);
                throw;
            }
        }

        _logger.LogWarning("Sale não encontrada. id:{0}", id);
        return null;
    }

    public void UpdateStatus(string saleId, SaleStatus status)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            UPDATE sales
            SET status = @status, updated_at = @updated_at
            WHERE id = @id;", conn);

        cmd.Parameters.AddWithValue("@status", (int)status);
        cmd.Parameters.AddWithValue("@id", saleId);
        cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

        cmd.ExecuteNonQuery();
        _logger.LogInformation("UpdateStatus -> id:{0}, status:{1}", saleId, (int)status);
    }

    public void CancelSale(string saleId) => UpdateStatus(saleId, SaleStatus.Canceled);

    public void AddItem(SaleItem item)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO sale_items(id, sell_id, product_id, quantity, created_at, updated_at)
                VALUES (@id, @sell_id, @product_id, @quantity, @created_at, @updated_at);
            ", conn);

            cmd.Transaction = transaction;
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@sell_id", item.SellId);
            cmd.Parameters.AddWithValue("@product_id", item.ProductId);
            cmd.Parameters.AddWithValue("@quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@created_at", item.CreatedAt);
            cmd.Parameters.AddWithValue("@updated_at", item.UpdatedAt);

            _logger.LogInformation(
                "Insert SaleItem -> id:{0}, sell_id:{1}, product_id:{2}, quantity:{3}, created_at:{4}, updated_at:{5}",
                item.Id, item.SellId, item.ProductId, item.Quantity, item.CreatedAt, item.UpdatedAt
            );

            cmd.ExecuteNonQuery();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError("Erro ao inserir SaleItem: {0}", ex);
            throw;
        }
    }

    public SaleItem? GetItemById(string itemId)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT id, sell_id, product_id, quantity, created_at, updated_at
            FROM sale_items
            WHERE id = @id;", conn);

        cmd.Parameters.AddWithValue("@id", itemId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            try
            {
                return new SaleItem
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    SellId = reader.GetString(reader.GetOrdinal("sell_id")),
                    ProductId = reader.GetString(reader.GetOrdinal("product_id")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao ler SaleItem do DB. id:{0}, Exception:{1}", itemId, ex);
                throw;
            }
        }

        _logger.LogWarning("SaleItem não encontrado. id:{0}", itemId);
        return null;
    }

    public void UpdateItemQuantity(string itemId, int quantity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            UPDATE sale_items
            SET quantity = @quantity
            WHERE id = @id;", conn);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@id", itemId);

        cmd.ExecuteNonQuery();
        _logger.LogInformation("UpdateItemQuantity -> id:{0}, quantity:{1}", itemId, quantity);
    }

    public List<SaleItem> GetItemsBySaleId(string saleId)
    {
        var items = new List<SaleItem>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT id, sell_id, product_id, quantity, created_at, updated_at
            FROM sale_items
            WHERE sell_id = @sell_id;", conn);
        cmd.Parameters.AddWithValue("@sell_id", saleId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            try
            {
                items.Add(new SaleItem
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    SellId = reader.GetString(reader.GetOrdinal("sell_id")),
                    ProductId = reader.GetString(reader.GetOrdinal("product_id")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao ler SaleItem do DB. saleId:{0}, Exception:{1}", saleId, ex);
                throw;
            }
        }

        _logger.LogInformation("GetItemsBySaleId -> saleId:{0}, count:{1}", saleId, items.Count);
        return items;
    }

    public List<Sale> GetSalesByProduct(string productId)
    {
        var sales = new List<Sale>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT DISTINCT sell_id
            FROM sale_items
            WHERE product_id = @product_id;", conn);
        cmd.Parameters.AddWithValue("@product_id", productId);

        using var reader = cmd.ExecuteReader();
        var saleIds = new List<string>();
        while (reader.Read())
            saleIds.Add(reader.GetString(0));
        reader.Close();

        foreach (var sid in saleIds)
        {
            var s = GetById(sid);
            if (s != null) sales.Add(s);
        }

        _logger.LogInformation("GetSalesByProduct -> productId:{0}, count:{1}", productId, sales.Count);
        return sales;
    }

    public int GetSalesCountByProductAndStatus(string productId, SaleStatus status)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT COUNT(DISTINCT si.sell_id) 
            FROM sale_items si
            JOIN sales s ON s.id = si.sell_id
            WHERE si.product_id = @product_id AND s.status = @status;", conn);

        cmd.Parameters.AddWithValue("@product_id", productId);
        cmd.Parameters.AddWithValue("@status", (int)status);

        var result = cmd.ExecuteScalar();
        int count = Convert.ToInt32(result);

        _logger.LogInformation("GetSalesCountByProductAndStatus -> productId:{0}, status:{1}, count:{2}",
            productId, (int)status, count);

        return count;
    }

    public List<Sale> GetSalesByStatus(SaleStatus status)
    {
        var list = new List<Sale>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
            SELECT id, client_id, status, created_at, updated_at
            FROM sales
            WHERE status = @status
            ORDER BY created_at DESC;", conn);
        cmd.Parameters.AddWithValue("@status", (int)status);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            try
            {
                var sale = new Sale
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    ClientId = reader.GetString(reader.GetOrdinal("client_id")),
                    Status = (SaleStatus)reader.GetInt32(reader.GetOrdinal("status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    Items = GetItemsBySaleId(reader.GetString(reader.GetOrdinal("id")))
                };
                list.Add(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao ler Sale do DB. status:{0}, Exception:{1}", (int)status, ex);
                throw;
            }
        }

        _logger.LogInformation("GetSalesByStatus -> status:{0}, count:{1}", (int)status, list.Count);
        return list;
    }
}


// BeginTransaction , Commit e Rollback =  proteção.
// Ela garante que um conjunto de operações no banco só vale se TUDO der certo.
// Se algo falhar no meio → tudo volta atrás (rollback). 
// Sempre no STORAGE, nunca na service.
// A service só coordena a lógica.
//  Quem mexe no banco é o Storage → então é o Storage que tem poder de transação.
//No Storage → ao gravar, converta enum para int  
// ao ler, converta int para enum 