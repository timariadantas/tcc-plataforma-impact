using CurrencyService.Service;
using CurrencyService.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configura Serilog para console + arquivo
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/currency.log", rollingInterval: RollingInterval.Day)
    .CreateLogger(); 
                                     
    builder.Host.UseSerilog(); // usa o Serilog no lugar do logger padrão

// Pegar a URL da API externa da variável de ambiente
var apiUrl = builder.Configuration["CurrencyApiUrl"] ?? "https://economia.awesomeapi.com.br/all";

// Registrar o Storage passando a URL
builder.Services.AddSingleton<IQuoteStorage>(sp =>
{
  var logger = sp.GetRequiredService<ILogger<QuoteStorage>>();
    return new QuoteStorage(apiUrl, logger);
});

// Registrar o Service
builder.Services.AddScoped<QuoteService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
