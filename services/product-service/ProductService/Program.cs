using ProductService.Domain;
using ProductService.Domain.Validation;
using ProductService.Logging;
using ProductService.Service;
using ProductService.Storage;
using ProductService.DTO.Requests;
using ProductService.DTO.Responses;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ===== Carrega .env se não estiver no Docker =====
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
if (!isDocker)
{
    DotEnv.Load();
}

// ===== Configura connection string =====
var host = isDocker ? "product-db" : Environment.GetEnvironmentVariable("PRODUCT_DB_HOST");
var port = isDocker ? "5432" : Environment.GetEnvironmentVariable("PRODUCT_DB_PORT");
var user = Environment.GetEnvironmentVariable("PRODUCT_DB_USER");
var password = Environment.GetEnvironmentVariable("PRODUCT_DB_PASSWORD");
var database = Environment.GetEnvironmentVariable("PRODUCT_DB_NAME");

var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

// ===== Configura LoggerService =====
builder.Services.AddSingleton<LoggerService>(sp =>
    isDocker ? new LoggerService() : new LoggerService("logs/product-logs.txt")
);

// ===== Configura ProductStorage =====
builder.Services.AddSingleton<IProductStorage>(sp => new ProductStorage(connectionString));

// ===== Configura ProductsService com DI =====
builder.Services.AddSingleton<IProductsService>(sp =>
{
    var storage = sp.GetRequiredService<IProductStorage>();
    var logger = sp.GetRequiredService<LoggerService>();
    return new ProductsService(storage, logger);
});

// ===== Controllers e Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Middleware =====
if (app.Environment.IsDevelopment() || isDocker)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ===== Endpoint raiz opcional =====
app.MapGet("/", () => "API ProductService funcionando!");

app.Run();
