using System;
using System.Diagnostics;
using CurrencyService.Logging;
using CurrencyService.Service;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyService.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly QuoteService _service;
        private readonly LoggerService _logger;

        public CurrencyController(QuoteService service, LoggerService logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var quotes = _service.GetAll(); // pega todas as moedas do cache

                sw.Stop();
                _logger.Log("INFO", $"GET /api/currency retornou {quotes?.Count() ?? 0} cotações");
                return Ok(new
                {
                    message = "Cotação das moedas",
                    timestamp = DateTime.UtcNow,
                    elapsed = sw.ElapsedMilliseconds,
                    data = quotes,
                    error = ""
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Log("ERROR", $"Erro em GET /api/currency: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "Erro ao obter cotações",
                    timestamp = DateTime.UtcNow,
                    elapsed = sw.ElapsedMilliseconds,
                    data = (object?)null,
                    error = ex.Message
                });
            }
        }

        [HttpGet("{code}")]
        public IActionResult GetOne(string code)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var quote = _service.GetByCode(code.ToUpper());
                sw.Stop();

                if (quote == null)
                {
                    _logger.Log("WARNING", $"Moeda {code} não encontrada");
                    return NotFound(new
                    {
                        message = $"Cotação '{code}' não encontrada",
                        timestamp = DateTime.UtcNow,
                        elapsed = sw.ElapsedMilliseconds,
                        data = (object?)null,
                        error = "Não encontrada"
                    });
                }

                _logger.Log("INFO", $"GET /api/currency/{code} retornou cotação");
                return Ok(new
                {
                    message = "Cotação da moeda",
                    timestamp = DateTime.UtcNow,
                    elapsed = sw.ElapsedMilliseconds,
                    data = quote,
                    error = ""
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Log("ERROR", $"Erro em GET /api/currency/{code}: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "Erro ao obter cotação",
                    timestamp = DateTime.UtcNow,
                    elapsed = sw.ElapsedMilliseconds,
                    data = (object?)null,
                    error = ex.Message
                });
            }
        }
    }
}
