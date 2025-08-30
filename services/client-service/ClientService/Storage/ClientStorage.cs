using System.Data;
using System.Runtime.CompilerServices;
using ClientService.Domain;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace ClientService.Storage
{
    public class ClientStorage : IClientStorage
    {
        private readonly string _connectionString;

        public ClientStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Client client)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
            INSERT INTO clients(id, name , surname, email, birthdate , created_at , updated_at, active) VALUES (@id, @name, @surname, @email, @birthdate, @created_at, @updated_at, @active)", conn);

            cmd.Parameters.AddWithValue("id", client.Id);
            cmd.Parameters.AddWithValue("name", client.Name);
            cmd.Parameters.AddWithValue("surname", client.Surname);
            cmd.Parameters.AddWithValue("email", client.Email);
            cmd.Parameters.AddWithValue("birthdate", client.Birthdate);
            cmd.Parameters.AddWithValue("created_at", client.CreatedAt);
            cmd.Parameters.AddWithValue("updated_at", client.UpdatedAt);
            cmd.Parameters.AddWithValue("active", client.Active);

            cmd.ExecuteNonQuery();

        }

        public Client? GetById(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();
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
        public List<Client> GetAll()
        {

            var list = new List<Client>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM clients", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Client
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Surname = reader.GetString(reader.GetOrdinal("surname")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Birthdate = reader.GetDateTime(reader.GetOrdinal("Birthdate")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("Updated_at")),
                    Active = reader.GetBoolean(reader.GetOrdinal("active")),
                });
            }
            return list;
        }

        public void Delete(string id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM clients WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

       
    }
}
     

// Anotações IPC
// CRUD - CREATE - READ - UPDATE - DELETE
//Repository Pattern: ClientStorage  isolando acesso ao banco.
//Separation of Concerns: Storage não tem lógica de negócio.
//SQL puro (sem ORM)