using CartService.Storage;
using CartService.Service;
using CartService.Logging;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Carrega variáveis do ambiente (.env)
DotNetEnv.Env.Load();

// Connection string
var configuration = builder.Configuration;
var connectionString = $"Host={configuration["CART_DB_HOST"]};Port={configuration["CART_DB_PORT"]};Username={configuration["CART_DB_USER"]};Password={configuration["CART_DB_PASSWORD"]};Database={configuration["CART_DB_NAME"]}";

// Logger
var logger = new LoggerService("/app/Logs/cart-logfile.log");

// Injeção de dependências
builder.Services.AddSingleton(logger); // logger
builder.Services.AddSingleton<ISalesStorage>(new SalesStorage(connectionString));
builder.Services.AddScoped<ISalesService, SalesService>();

// Swagger e Controllers
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CartService API v1");
    c.RoutePrefix = "swagger/cart";
});

app.MapControllers();
app.Run();
