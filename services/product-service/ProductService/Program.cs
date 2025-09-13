using ProductService.Service;
using ProductService.Storage;
using ProductService.Logging;

var builder = WebApplication.CreateBuilder(args);

// Detecta se está rodando dentro do container
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Configura LoggerService
LoggerService logger;
if (isDocker)
{
    logger = new LoggerService(); // logs apenas em memória
}
else
{
    logger = new LoggerService("logs/product-logs.txt"); // logs em arquivo + memória
}

// Pega a connection string do ambiente ou appsettings(não tem)
string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                          ?? builder.Configuration.GetConnectionString("DefaultConnection")
                          ?? throw new InvalidOperationException("Connection string não definida");

// Registrar Storage
builder.Services.AddSingleton<IProductStorage>(sp => new ProductStorage(connectionString));

// Registrar Logger
builder.Services.AddSingleton(logger);

// Registrar Service passando Logger
builder.Services.AddSingleton<IProductsService>(sp =>
{
    var storage = sp.GetRequiredService<IProductStorage>();
    var log = sp.GetRequiredService<LoggerService>();
    return new ProductsService(storage, log);
});

// Configura Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();


//Registramos LoggerService como singleton.

//Ao registrar o ProductsService, pegamos IProductStorage e LoggerService do container e passamos para o construtor.

//Mantemos compatibilidade com Docker (memória) e local (arquivo + memória).