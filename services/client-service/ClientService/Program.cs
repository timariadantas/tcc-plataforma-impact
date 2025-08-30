using ClientService.Storage;
using ClientService.Service;

var builder = WebApplication.CreateBuilder(args);

// Pega a connection string do ambiente
string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                          ?? throw new InvalidOperationException("Connection string não definida");

// Injeção de dependência
builder.Services.AddSingleton<IClientStorage>(new ClientStorage(connectionString));
builder.Services.AddScoped<IClientService, ClientService.Service.ClientService>();

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
