using Microsoft.AspNetCore.Mvc;
using ProductService.Service;          
using ProductService.Domain;           
using ProductService.DTO.Requests;
using ProductService.DTO.Responses;
using ProductService.Logging;  
using System.Diagnostics;
using System.ComponentModel;


namespace ProductService.Controllers
{[ApiController]
[Route("products")]  // <-- prefixo padrão definido pelo professor
public class ProductController : ControllerBase
{
    private readonly IProductsService _service;
    private readonly LoggerService _logger;

    public ProductController(IProductsService service, LoggerService logger)
    {
        _service = service;
        _logger = logger;
    }
[HttpPost] // cria produto
public IActionResult Create([FromBody] CreateProductDto dto)
{
    var stopwatch = Stopwatch.StartNew();
    var response = new ApiResponse<ProductResponseDto>();

    try
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity
        };

        var created = _service.Create(product);

        response.Data = new ProductResponseDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price,
            Quantity = created.Quantity
        };
        response.Message = "Produto criado com sucesso";
    }
    catch (Exception ex)
    {
        response.Error = ex.Message;
        response.Message = "Erro ao criar produto";
        _logger.Log(ex.Message, Logging.LogLevel.ERROR);
        return StatusCode(500, response);
    }
    finally
    {
        stopwatch.Stop();
        response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
        response.Timestamp = DateTime.UtcNow;
    }

    return Ok(response);
}

[HttpPut("{id}")] // atualiza produto
public IActionResult Update(string id, [FromBody] CreateProductDto dto)
{
    var stopwatch = Stopwatch.StartNew();
    var response = new ApiResponse<ProductResponseDto>();

    try
    {
        var updated = _service.UpdateProd(id, dto.Name, dto.Description);
        updated = _service.UpdatePrice(id, dto.Price);
        updated = _service.UpdateQuantity(id, dto.Quantity);

        response.Data = new ProductResponseDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price,
            Quantity = updated.Quantity
        };
        response.Message = "Produto atualizado com sucesso";
    }
    catch (Exception ex)
    {
        response.Error = ex.Message;
        response.Message = "Erro ao atualizar produto";
        _logger.Log(ex.Message, Logging.LogLevel.ERROR);
        return StatusCode(500, response);
    }
    finally
    {
        stopwatch.Stop();
        response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
        response.Timestamp = DateTime.UtcNow;
    }

    return Ok(response);
}

[HttpDelete("{id}")] // remove produto
public IActionResult Delete(string id)
{
    var stopwatch = Stopwatch.StartNew();
    var response = new ApiResponse<string>();

    try
    {
        _service.Delete(id);
        response.Message = "Produto removido com sucesso";
        response.Data = id;
    }
    catch (Exception ex)
    {
        response.Error = ex.Message;
        response.Message = "Erro ao remover produto";
        _logger.Log(ex.Message, Logging.LogLevel.ERROR);
        return StatusCode(500, response);
    }
    finally
    {
        stopwatch.Stop();
        response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
        response.Timestamp = DateTime.UtcNow;
    }

    return Ok(response);
}

    [HttpGet]
    public IActionResult GetAll()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new ApiResponse<List<ProductResponseDto>>();

        try
        {
            var products = _service.GetAll();
            response.Data = products.Select(p => new ProductResponseDto {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity
            }).ToList();
            response.Message = "Lista de produtos recuperada com sucesso";
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            response.Message = "Erro interno ao recuperar produtos";
            _logger.Log(ex.Message, Logging.LogLevel.ERROR);
            return StatusCode(500, response);
        }
        finally
        {
            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = DateTime.UtcNow;
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new ApiResponse<ProductResponseDto>();

        try
        {
            var product = _service.GetById(id);
            if (product == null)
            {
                response.Error = "Produto não encontrado";
                response.Message = "Produto não existe";
                return NotFound(response);
            }

            response.Data = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Quantity
            };
            response.Message = "Produto consultado com sucesso";
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            response.Message = "Erro interno";
            _logger.Log(ex.Message, Logging.LogLevel.ERROR);
            return StatusCode(500, response);
        }
        finally
        {
            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = DateTime.UtcNow;
        }

        return Ok(response);
    }

    
}

}

