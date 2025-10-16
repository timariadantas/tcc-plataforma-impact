using ClientService.Storage;
using ClientService.Service;
using ClientService.Logging;
using DotNetEnv;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Carrega .env localmente (fora do container)
Env.Load("/home/mariadantas/plataforma-tcc/.env");

// 🔹 Detecta se está rodando no Docker
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// 🔹 Pega as variáveis do ambiente
var host = isDocker ? "client-db" : Environment.GetEnvironmentVariable("CLIENT_DB_HOST");
var port = isDocker ? "5432" : Environment.GetEnvironmentVariable("CLIENT_DB_PORT");
var user = Environment.GetEnvironmentVariable("CLIENT_DB_USER");
var password = Environment.GetEnvironmentVariable("CLIENT_DB_PASSWORD");
var database = Environment.GetEnvironmentVariable("CLIENT_DB_NAME");

// 🔹 Monta connection string diretamente
var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

// 🔹 Logger
LoggerService logger = isDocker 
    ? new LoggerService("/app/Logs/logfile.log")
    : new LoggerService("logs/client-logs.txt");

// 🔹 Teste rápido de conexão
try
{
    using var conn = new NpgsqlConnection(connectionString);
    conn.Open();
    Console.WriteLine("✅ Conexão com o banco OK!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao conectar com o banco: {ex.Message}");
}

// 🔹 Injeção de dependências
builder.Services.AddSingleton<IClientStorage>(new ClientStorage(connectionString, logger));
builder.Services.AddScoped<IClientService, ClientService.Service.ClientService>();
builder.Services.AddSingleton(logger);

// 🔹 Swagger e Controllers
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientService API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();
app.Run();
