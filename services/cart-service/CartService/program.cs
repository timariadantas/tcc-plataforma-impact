using CartService.Storage;
using CartService.Service;
using CartService.Logging;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Carrega variáveis do .env (fora do container)
Env.Load("/home/mariadantas/plataforma-tcc/.env");

// 🔹 Detecta se está rodando dentro do container
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// 🔹 Monta host e porta conforme ambiente
var host = isDocker ? "cart-db" : Environment.GetEnvironmentVariable("CART_DB_HOST");
var port = isDocker ? "5432" : Environment.GetEnvironmentVariable("CART_DB_PORT");
var user = Environment.GetEnvironmentVariable("CART_DB_USER");
var password = Environment.GetEnvironmentVariable("CART_DB_PASSWORD");
var database = Environment.GetEnvironmentVariable("CART_DB_NAME");

// 🔹 Monta connection string
string connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

// 🔹 Configura LoggerService
LoggerService logger = isDocker
    ? new LoggerService()                   // logs apenas em memória (Docker)
    : new LoggerService("logs/cart-logs.txt"); // logs em arquivo local

// 🔹 Injeção de dependência
builder.Services.AddSingleton<ISalesStorage>(new SalesStorage(connectionString));
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddSingleton(logger);

// 🔹 Swagger e Controllers
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:8080"); // Porta do container

var app = builder.Build();

// 🔹 Configura Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CartService API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();
app.Run();
