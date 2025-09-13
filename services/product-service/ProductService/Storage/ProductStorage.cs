using System.Collections.Generic;
using Npgsql;
using NUlid;
using ProductService.Domain;

namespace ProductService.Storage
{
    public class ProductStorage : IProductStorage
    {
        private readonly string _connectionString;

        public ProductStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Product product)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO products (id, name, description, price, quantity, created_at, updated_at, active)
                VALUES (@id, @name, @description, @price, @quantity, @created_at, @updated_at, @active)", conn);

            if (string.IsNullOrEmpty(product.Id))
                product.Id = Ulid.NewUlid().ToString();

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            cmd.Parameters.AddWithValue("id", product.Id);
            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("created_at", product.CreatedAt);
            cmd.Parameters.AddWithValue("updated_at", product.UpdatedAt);
            cmd.Parameters.AddWithValue("active", product.Active);

            cmd.ExecuteNonQuery();
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM products WHERE active = true", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetFloat(reader.GetOrdinal("price")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    Active = reader.GetBoolean(reader.GetOrdinal("active"))
                });
            }

            return products;
        }

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
                Price = reader.GetFloat(reader.GetOrdinal("price")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                Active = reader.GetBoolean(reader.GetOrdinal("active"))
            };
        }

        public void Delete(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM products WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        public void UpdateProd(string id, string name, string description)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE products SET name=@name, description=@description WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("description", description);
            cmd.ExecuteNonQuery();

        }

        public void UpdatePrice(string id, decimal price)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE products SET price=@price WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("price", price);
            cmd.ExecuteNonQuery();
        }

        public void UpdateQuantity(string id, int quantity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE products SET quantity=@quantity WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("quantity", quantity);
            cmd.ExecuteNonQuery();
        }


        public void Inactivate(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE products SET active=false WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();

        }
    }
}