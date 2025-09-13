using Microsoft.AspNetCore.Mvc;
using ProductService.Domain;
using ProductService.Service;
using ProductService.Logging; // Certifique-se de que LoggerService está nesse namespace

namespace ProductService.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductsService _service;
        private readonly LoggerService _logger;

        public ProductController(IProductsService service, LoggerService logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            try
            {
                _service.Create(product);
                _logger.Log($"Produto criado: {product.Name}");
                return Ok(new {
                    message = "Produto criado com sucesso",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao criar produto: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var product = _service.GetById(id);
                if (product == null)
                {
                    _logger.Log($"Produto não encontrado: {id}");
                    return NotFound(new { error = "Produto não encontrado", timestamp = DateTime.UtcNow });
                }

                _logger.Log($"Produto consultado: {id}");
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao buscar produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var products = _service.GetAll();
                _logger.Log($"Consulta de todos os produtos ({products.Count})");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao listar produtos: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _service.Delete(id);
                _logger.Log($"Produto removido: {id}");
                return Ok(new { message = "Produto removido permanentemente", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao remover produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProd(string id, [FromBody] Product body)
        {
            try
            {
                _service.UpdateProd(id, body.Name, body.Description);
                _logger.Log($"Produto atualizado (nome/descrição): {id}");
                return Ok(new { message = "Produto atualizado", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPatch("{id}/price")]
        public IActionResult UpdatePrice(string id, [FromBody] decimal price)
        {
            try
            {
                _service.UpdatePrice(id, price);
                _logger.Log($"Preço do produto atualizado: {id} -> {price}");
                return Ok(new { message = "Preço atualizado", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar preço do produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPatch("{id}/quantity")]
        public IActionResult UpdateQuantity(string id, [FromBody] int quantity)
        {
            try
            {
                _service.UpdateQuantity(id, quantity);
                _logger.Log($"Quantidade do produto atualizada: {id} -> {quantity}");
                return Ok(new { message = "Estoque atualizado", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar estoque do produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }

        [HttpPatch("{id}/inactivate")]
        public IActionResult Inactivate(string id)
        {
            try
            {
                _service.Inactivate(id);
                _logger.Log($"Produto inativado: {id}");
                return Ok(new { message = "Produto inativado", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao inativar produto {id}: {ex.Message}");
                return BadRequest(new { error = ex.Message, timestamp = DateTime.UtcNow });
            }
        }
    }
}
