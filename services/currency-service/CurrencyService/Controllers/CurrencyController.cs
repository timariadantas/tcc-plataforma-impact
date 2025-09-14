using Microsoft.AspNetCore.Mvc;
using CurrencyService.Service;
using Microsoft.Extensions.Logging;

namespace CurrencyService.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly QuoteService _service;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(QuoteService service, ILogger<CurrencyController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/currency chamado");
            var quotes = await _service.GetQuotesAsync();
            return Ok(new
            {
                message = "Cotação das moedas",
                timestamp = DateTime.UtcNow,
                data = quotes
            });
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetOne(string code)
        {
            _logger.LogInformation("GET /api/currency/{code} chamado", code);
            var quote = await _service.GetQuoteAsync(code);
            if (quote is null)
            {
                _logger.LogWarning("Moeda {Code} não encontrada", code);
                return NotFound(new { message = $"Cotação '{code}' não encontrada" });
            }

            return Ok(new
            {
                message = "Cotação da moeda",
                timestamp = DateTime.UtcNow,
                data = quote
            });
        }
    }
}

