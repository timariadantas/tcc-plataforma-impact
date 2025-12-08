using ProductService.Storage;
using ProductService.Service;
using ProductService.Logging;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Carrega variáveis do ambiente (.env)
DotNetEnv.Env.Load();


// Connection string
var configuration = builder.Configuration;
var connectionString = $"Host={configuration["PRODUCT_DB_HOST"]};Port={configuration["PRODUCT_DB_PORT"]};Username={configuration["PRODUCT_DB_USER"]};Password={configuration["PRODUCT_DB_PASSWORD"]};Database={configuration["PRODUCT_DB_NAME"]}";

// Logger
var logger = new LoggerService("/app/Logs/product-logfile.log");

// Injeção de dependências
builder.Services.AddSingleton(logger); // logger
builder.Services.AddSingleton<IProductStorage>(new ProductStorage(connectionString, logger));
builder.Services.AddScoped<IProductsService, ProductsService>();

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductService API v1");
    c.RoutePrefix = "swagger/product";
});

app.MapControllers();
app.Run();
