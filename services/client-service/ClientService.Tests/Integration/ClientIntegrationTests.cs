using Xunit;
using ClientService.Storage;
using ClientService.Domain;
using System;
using System.IO;
using DotNetEnv;
using ClientService.Logging;

namespace ClientService.IntegrationTests
{
    public class ClientIntegrationTests
    {
        private readonly ClientStorage _storage;

        public ClientIntegrationTests()

        {
            // 🔹 Carrega variáveis do .env (para rodar fora do container)
            Env.Load("/home/mariadantas/plataforma-tcc/.env");

            // Detecta se o teste está rodando dentro do container
            bool runningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            // Configura host e porta dinamicamente
            string host = runningInDocker 
                ? "client-db" 
                : Environment.GetEnvironmentVariable("CLIENT_DB_HOST");

            string port = runningInDocker 
                ? "5432" 
                : Environment.GetEnvironmentVariable("CLIENT_DB_PORT");

            string user = Environment.GetEnvironmentVariable("CLIENT_DB_USER");
            string password = Environment.GetEnvironmentVariable("CLIENT_DB_PASSWORD");
            string database = Environment.GetEnvironmentVariable("CLIENT_DB_NAME");

            // Monta a connection string
            string connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

            //  Cria o logger necessário para o ClientStorage
            var logger = new LoggerService();

            _storage = new ClientStorage(connectionString , logger);
        }

        [Fact]
        public void CreateClient_ShouldPersistInDatabase()
        {
            // Arrange
            var client = new Client
            {
                Name = "TestIntegration",
                Surname = "User",
                Email = "integration@test.com",
                Birthdate = new DateTime(1990, 1, 1)
            };

            // Act
            _storage.Create(client);
            var clients = _storage.GetAll();

            // Assert
            Assert.Contains(clients, c => c.Email == "integration@test.com");
        }
    }
}
