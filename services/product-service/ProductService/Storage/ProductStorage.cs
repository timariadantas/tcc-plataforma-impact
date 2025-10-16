using ProductService.Domain;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ProductService.Storage
{
    public class ProductStorage : IProductStorage
    {
        private readonly string _connectionString;

        public ProductStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        // CREATE
        public void Create(Product product)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO products(name, description, price, quantity)
                VALUES (@name, @description, @price, @quantity)
                RETURNING id, created_at, updated_at, active", conn);

            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                product.Id = reader.GetString(reader.GetOrdinal("id"));
                product.CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                product.UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"));
                product.Active = reader.GetBoolean(reader.GetOrdinal("active"));
            }
        }

        // GET BY ID
        public Product? GetById(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM products WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Product
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                Active = reader.GetBoolean(reader.GetOrdinal("active")),
            };
        }

        // GET ALL
        public List<Product> GetAll()
        {
            var list = new List<Product>();

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM products WHERE active = true", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Product
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    Active = reader.GetBoolean(reader.GetOrdinal("active")),
                });
            }

            return list;
        }

        // DELETE
        public void Delete(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM products WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

        // UPDATE
        public void Update(Product product)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                UPDATE products
                SET name = @name, description = @description, price = @price, quantity = @quantity, active = @active
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", product.Id);
            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("active", product.Active);

            cmd.ExecuteNonQuery();
        }

        public void UpdateProd(string id, string name, string description)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
        UPDATE products
        SET name = @name, description = @description, updated_at = NOW()
        WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("description", description);

            cmd.ExecuteNonQuery();
        }

        public void UpdatePrice(string id, decimal price)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
        UPDATE products
        SET price = @price, updated_at = NOW()
        WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("price", price);

            cmd.ExecuteNonQuery();
        }

        public void UpdateQuantity(string id, int quantity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
        UPDATE products
        SET quantity = @quantity, updated_at = NOW()
        WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("quantity", quantity);

            cmd.ExecuteNonQuery();
        }

        public void Inactivate(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
        UPDATE products
        SET active = false, updated_at = NOW()
        WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

    }
}