using CurrencyService.Service;
using CurrencyService.Storage;

var builder = WebApplication.CreateBuilder(args);

// Pegar a URL da API externa da variável de ambiente
var apiUrl = builder.Configuration["CurrencyApiUrl"] ?? "https://economia.awesomeapi.com.br/all";

// Registrar o Storage passando a URL
builder.Services.AddSingleton<IQuoteStorage>(sp => new QuoteStorage(apiUrl));

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
