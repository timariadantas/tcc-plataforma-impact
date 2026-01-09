using ClientService.Storage;
using ClientService.Service;
using ClientService.Logging;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Carrega variáveis do ambiente (.env já carregado pelo Docker ou localmente)
DotNetEnv.Env.Load();

// Connection string
var configuration = builder.Configuration;
var connectionString = $"Host={configuration["CLIENT_DB_HOST"]};Port={configuration["CLIENT_DB_PORT"]};Username={configuration["CLIENT_DB_USER"]};Password={configuration["CLIENT_DB_PASSWORD"]};Database={configuration["CLIENT_DB_NAME"]}";

// Logger
var logger = new LoggerService("/app/Logs/logfile.log");

// Injeção de dependências
builder.Services.AddSingleton<IClientStorage>(new ClientStorage(connectionString, logger));
builder.Services.AddScoped<IClientService, ClientService.Service.ClientService>();
builder.Services.AddSingleton(logger);


// Swagger e Controllers
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();
//fazer os logs aparecerem para cada requisição
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.Now}] {context.Request.Method} {context.Request.Path}");
    await next();
});

// Registrar middleware
app.UseMiddleware<ClientService.Middleware.ExceptionMiddleware>();
app.UseRouting(); 
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientService API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();
app.Run();
