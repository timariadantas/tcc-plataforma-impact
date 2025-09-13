using ClientService.Storage;
using ClientService.Service;
using ClientService.Logging;

var builder = WebApplication.CreateBuilder(args);

// LoggerService duplo docker (logs em memória)  Local (logs em arquivo + memória)


// Detecta se está rodando dentro do container
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

LoggerService logger;
if (isDocker)
{

    logger = new LoggerService();
}
else
{
    logger = new LoggerService("logs/client-logs.txt");
}


// Pega a connection string do ambiente
string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                          ?? throw new InvalidOperationException("Connection string não definida");

// Injeção de dependência
builder.Services.AddSingleton<IClientStorage>(new ClientStorage(connectionString));
builder.Services.AddScoped<IClientService, ClientService.Service.ClientService>();
builder.Services.AddSingleton(logger);

// Adiciona Swagger
builder.Services.AddSwaggerGen();

// Adiciona Controllers
builder.Services.AddControllers();

// Expõe a aplicação na porta 80 dentro do container
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// Configura Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientService API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:8000/
});

// Configura autorização e rotas
app.UseAuthorization();
app.MapControllers();

app.Run();
