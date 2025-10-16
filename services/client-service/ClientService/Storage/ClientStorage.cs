using ClientService.Domain;
using ClientService.Logging;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ClientService.Storage
{
    public class ClientStorage : IClientStorage
    {
        private readonly string _connectionString;
        private readonly LoggerService _logger;

        public ClientStorage(string connectionString, LoggerService logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        // CREATE
        public void Create(Client client)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open(); // síncrono

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO clients( name, surname, email, birthdate)
                VALUES ( @name, @surname, @email, @birthdate) RETURNING id, created_at, updated_at, active", conn);


            cmd.Parameters.AddWithValue("name", client.Name);
            cmd.Parameters.AddWithValue("surname", client.Surname);
            cmd.Parameters.AddWithValue("email", client.Email);
            cmd.Parameters.AddWithValue("birthdate", client.Birthdate);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                client.Id = reader.GetString(reader.GetOrdinal("id"));
                client.CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                client.UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"));
                client.Active = reader.GetBoolean(reader.GetOrdinal("active"));
            }

        }

        // GET BY ID
        public Client? GetById(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open(); // síncrono

            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader(); // síncrono
            if (!reader.Read()) return null;

            return new Client
            {
                Id = reader.GetString(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Surname = reader.GetString(reader.GetOrdinal("surname")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Birthdate = reader.GetDateTime(reader.GetOrdinal("birthdate")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                Active = reader.GetBoolean(reader.GetOrdinal("active")),
            };
        }

        // GET ALL
        public List<Client> GetAll()
        {
            var list = new List<Client>();

            try
            {

                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open(); // síncrono

                _logger.Log("Conexão aberta com sucesso em GetAll.");

                using var cmd = new NpgsqlCommand("SELECT * FROM clients", conn);
                using var reader = cmd.ExecuteReader(); // síncrono

                while (reader.Read())
                {
                    list.Add(new Client
                    {
                        Id = reader.GetString(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Surname = reader.GetString(reader.GetOrdinal("surname")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Birthdate = reader.GetDateTime(reader.GetOrdinal("birthdate")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                        Active = reader.GetBoolean(reader.GetOrdinal("active")),
                    });
                }
                _logger.Log($"GetAll finalizado. Total de clientes: {list.Count}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro no ClientStorage.GetAll: {ex.GetType().Name} - {ex.Message}");
                _logger.Log(ex.StackTrace ?? "Sem stack trace");
                throw;
            }
            return list;
        }
    
        // DELETE
        public void Delete(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open(); // síncrono

            using var cmd = new NpgsqlCommand("DELETE FROM clients WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery(); // síncrono
        }

        // UPDATE (opcional)
        public void Update(Client client)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                UPDATE clients 
                SET name = @name, surname = @surname, email = @email, birthdate = @birthdate,  active = @active
                WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", client.Id);
            cmd.Parameters.AddWithValue("name", client.Name);
            cmd.Parameters.AddWithValue("surname", client.Surname);
            cmd.Parameters.AddWithValue("email", client.Email);
            cmd.Parameters.AddWithValue("birthdate", client.Birthdate);
            cmd.Parameters.AddWithValue("active", client.Active);

            cmd.ExecuteNonQuery();
        }

    }
}
