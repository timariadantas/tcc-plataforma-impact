using CartService.Storage;
using CartService.Service;
using CartService.Logging;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;

// variáveis no docker-compose
var connectionString =
    $"Host={configuration["CART_DB_HOST"]};" +
    $"Port={configuration["CART_DB_PORT"]};" +
    $"Username={configuration["CART_DB_USER"]};" +
    $"Password={configuration["CART_DB_PASSWORD"]};" +
    $"Database={configuration["CART_DB_NAME"]}";


builder.Services.AddSingleton<ILoggerService>(sp =>
    new LoggerService("/app/Logs/cart.log"));


builder.Services.AddSingleton<ILoggerService>(sp =>  //servidorprovider
    new LoggerService("/app/Logs/cart.log"));



//DI
builder.Services.AddSingleton<ISalesStorage>(sp =>
    new SalesStorage(connectionString, sp.GetRequiredService<ILoggerService>())
);

builder.Services.AddScoped<ISalesService, SalesService>();


builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Porta de Conf Serviço
builder.WebHost.UseUrls("http://0.0.0.0:8080");


var app = builder.Build();

// Middleware padrão
app.UseRouting();
app.UseAuthorization();

// Swagger 
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CartService API v1");
    c.RoutePrefix = "swagger/cart";
});

// Mapear controllers
app.MapControllers();

// Rodar aplicação
app.Run();
