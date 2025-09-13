using Microsoft.AspNetCore.Mvc;
using CurrencyService.Service;

namespace CurrencyService.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly QuoteService _service;

        public CurrencyController(QuoteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quotes = await _service.GetQuotesAsync();
            return Ok(new
            {
                message = "Cotação das moedas",
                timestamp = DateTime.UtcNow,
                elapsed = 0,
                data = quotes
            });
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetOne(string code)
        {
            var quote = await _service.GetQuoteAsync(code.ToUpper());
            if (quote is null)
                return NotFound(new { message = $"Cotação '{code}' não encontrada" });

            return Ok(new
            {
                message = "Cotação da moeda",
                timestamp = DateTime.UtcNow,
                elapsed = 0,
                data = quote
            });
        }
    }
}
