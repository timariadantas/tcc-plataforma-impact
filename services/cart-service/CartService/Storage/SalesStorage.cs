using System;
using System.Collections.Generic;
using Npgsql;
using CartService.Domain;

namespace CartService.Storage;

public class SalesStorage : ISalesStorage
{
    private readonly string _connectionString;

    public SalesStorage(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(Sale sale)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO sales(client_id, status)
                VALUES (@client_id, @status)
                RETURNING id, created_at, updated_at;", conn);

            cmd.Parameters.AddWithValue("client_id", sale.ClientId);
            cmd.Parameters.AddWithValue("status", sale.Status);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sale.Id = reader.GetString(0);
                sale.CreatedAt = reader.GetDateTime(1);
                sale.UpdatedAt = reader.GetDateTime(2);
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
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
        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var sale = new Sale
            {
                Id = reader.GetString(0),
                ClientId = reader.GetString(1),
                Status = reader.GetInt32(2),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4),
                Items = GetItemsBySaleId(id)
            };
            return sale;
        }

        return null;
    }

    public void UpdateStatus(string saleId, int status)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
                UPDATE sales
                SET status = @status
                WHERE id = @id;", conn);
        cmd.Parameters.AddWithValue("status", status);
        cmd.Parameters.AddWithValue("id", saleId);

        cmd.ExecuteNonQuery();
    }

    public void CancelSale(string saleId)
    {
        UpdateStatus(saleId, (int)SaleStatus.Canceled);
    }

    public void AddItem(SaleItem item)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {

        using var cmd = new NpgsqlCommand(@"
                INSERT INTO sale_items(sell_id, product_id, quantity)
                VALUES (@sell_id, @product_id, @quantity)
                RETURNING id, created_at, updated_at;", conn);

        cmd.Parameters.AddWithValue("sell_id", item.SellId);
        cmd.Parameters.AddWithValue("product_id", item.ProductId);
        cmd.Parameters.AddWithValue("quantity", item.Quantity);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            item.Id = reader.GetString(0);
            item.CreatedAt = reader.GetDateTime(1);
            item.UpdatedAt = reader.GetDateTime(2);
        }
        transaction.Commit();
        }
        catch
        {
        transaction.Rollback();
        throw;
        }

    }

    public void UpdateItemQuantity(string itemId, int quantity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
                UPDATE sale_items
                SET quantity = @quantity
                WHERE id = @id;", conn);
        cmd.Parameters.AddWithValue("quantity", quantity);
        cmd.Parameters.AddWithValue("id", itemId);

        cmd.ExecuteNonQuery();
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
        cmd.Parameters.AddWithValue("sell_id", saleId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new SaleItem
            {
                Id = reader.GetString(0),
                SellId = reader.GetString(1),
                ProductId = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                CreatedAt = reader.GetDateTime(4),
                UpdatedAt = reader.GetDateTime(5)
            });
        }

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
        cmd.Parameters.AddWithValue("product_id", productId);

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

        return sales;
    }

    public int GetSalesCountByProductAndStatus(string productId, int status)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
                SELECT COUNT(DISTINCT si.sell_id) 
                FROM sale_items si
                JOIN sales s ON s.id = si.sell_id
                WHERE si.product_id = @product_id AND s.status = @status;", conn);

        cmd.Parameters.AddWithValue("product_id", productId);
        cmd.Parameters.AddWithValue("status", status);

        var result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public List<Sale> GetSalesByStatus(int status)
    {
        var list = new List<Sale>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(@"
                SELECT id, client_id, status, created_at, updated_at
                FROM sales
                WHERE status = @status
                ORDER BY created_at DESC;", conn);
        cmd.Parameters.AddWithValue("status", status);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var sale = new Sale
            {
                Id = reader.GetString(0),
                ClientId = reader.GetString(1),
                Status = reader.GetInt32(2),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4),
                Items = GetItemsBySaleId(reader.GetString(0))
            };
            list.Add(sale);
        }

        return list;
    }
}

// BeginTransaction , Commit e Rollback =  proteção.
// Ela garante que um conjunto de operações no banco só vale se TUDO der certo.
// Se algo falhar no meio → tudo volta atrás (rollback). 
// Sempre no STORAGE, nunca na service.
// A service só coordena a lógica.
//  Quem mexe no banco é o Storage → então é o Storage que tem poder de transação.