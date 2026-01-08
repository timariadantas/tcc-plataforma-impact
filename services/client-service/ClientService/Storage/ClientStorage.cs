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

            // Começa a transação
            using var transaction = conn.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand(@"
            INSERT INTO clients(id, name, surname, email, birthdate)
            VALUES (@id, @name, @surname, @email, @birthdate)",
             conn, transaction);

                cmd.Parameters.AddWithValue("id", client.Id);
                cmd.Parameters.AddWithValue("name", client.Name);
                cmd.Parameters.AddWithValue("surname", client.Surname);
                cmd.Parameters.AddWithValue("email", client.Email);
                cmd.Parameters.AddWithValue("birthdate", client.Birthdate);

                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Log($"Erro ao criar cliente: {ex.GetType().Name} - {ex.Message}");
               // _logger.Log(ex.StackTrace ?? "Sem stack trace"); 
                throw;
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

            return Client.Restore(
            
                reader.GetString(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetString(reader.GetOrdinal("surname")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.GetDateTime(reader.GetOrdinal("birthdate")),
                reader.GetDateTime(reader.GetOrdinal("created_at")),
                reader.GetDateTime(reader.GetOrdinal("updated_at")),
                reader.GetBoolean(reader.GetOrdinal("active"))
            );
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
                    list.Add(Client.Restore(
                    
                       reader.GetString(reader.GetOrdinal("id")),
                       reader.GetString(reader.GetOrdinal("name")),
                        reader.GetString(reader.GetOrdinal("surname")),
                      reader.GetString(reader.GetOrdinal("email")),
                       reader.GetDateTime(reader.GetOrdinal("birthdate")),
                       reader.GetDateTime(reader.GetOrdinal("created_at")),
                      reader.GetDateTime(reader.GetOrdinal("updated_at")),
                     reader.GetBoolean(reader.GetOrdinal("active"))
                    ));
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
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand("DELETE FROM clients WHERE id = @id", conn, transaction);

                cmd.Parameters.AddWithValue("id", id);

                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Log ($"Erro ao deletar cliente: {ex.Message}");
                throw;
            }
        }

        // UPDATE (opcional)

        public void Update(Client client)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand(@"
                    UPDATE clients 
                    SET name = @name, surname = @surname, email = @email, birthdate = @birthdate, active = @active
                    WHERE id = @id", conn, transaction);

                cmd.Parameters.AddWithValue("id", client.Id);
                cmd.Parameters.AddWithValue("name", client.Name);
                cmd.Parameters.AddWithValue("surname", client.Surname);
                cmd.Parameters.AddWithValue("email", client.Email);
                cmd.Parameters.AddWithValue("birthdate", client.Birthdate);
                cmd.Parameters.AddWithValue("active", client.Active);

                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Log($"Erro ao atualizar cliente: {ex.Message}");
                throw;
            }
        }
    }
}