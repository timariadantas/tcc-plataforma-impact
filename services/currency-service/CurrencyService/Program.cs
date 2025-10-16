using CurrencyService.Storage;
using CurrencyService.Service;
using CurrencyService.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------
// LoggerService (Singleton para toda a aplicação)
builder.Services.AddSingleton<LoggerService>(sp =>
{
    // Pode passar um caminho de arquivo para salvar logs, se quiser
    return new LoggerService();
});

// -------------------------------
// URL da API externa
var apiUrl = builder.Configuration["CurrencyApiUrl"] ?? "https://economia.awesomeapi.com.br/all";

// -------------------------------
// QuoteStorage (Singleton com User-Agent)
builder.Services.AddSingleton<IQuoteStorage>(sp =>
{
    var logger = sp.GetRequiredService<LoggerService>();
    return new QuoteStorage(apiUrl, logger);
});

// -------------------------------
// QuoteService (Singleton)
builder.Services.AddSingleton<QuoteService>();

// -------------------------------
// Adiciona Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------------
// Middleware Swagger e HTTPS
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapControllers();

// -------------------------------
// Configura para aceitar qualquer IP (importante para Docker)
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5000");

// -------------------------------
// Inicializa aplicação
app.Run();
