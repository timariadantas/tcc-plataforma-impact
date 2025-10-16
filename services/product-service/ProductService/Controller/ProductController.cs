using Microsoft.AspNetCore.Mvc;
using ProductService.Service;          
using ProductService.Domain;           
using ProductService.DTO.Requests;     
using ProductService.DTO.Responses;    

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")] 
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _service;

        public ProductsController(IProductsService service)
        {
            _service = service;
        }

       
        // Criar produto
        [HttpPost]
        public ActionResult<ProductResponseDto> Create([FromBody] CreateProductDto dto)
        {
            // Converte DTO de entrada para domínio
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                
            };

            // Chama serviço para criar
            var created = _service.Create(product);

            // Converte domínio para DTO de saída
            var response = new ProductResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                Quantity = created.Quantity,
                Active = created.Active
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

       
        // Listar todos produtos
        
        [HttpGet]
        public ActionResult<List<ProductResponseDto>> GetAll()
        {
            var products = _service.GetAll();

            var response = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                Active = p.Active
            }).ToList();

            return Ok(response);
        }

        // Buscar produto por ID
       
        [HttpGet("{id}")]
        public ActionResult<ProductResponseDto> GetById(string id)
        {
            var product = _service.GetById(id);
            if (product == null) return NotFound();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Active = product.Active
            };

            return Ok(response);
        }

       
        // Atualizar nome e descrição
        
        [HttpPut("{id}")]
        public ActionResult<ProductResponseDto> UpdateProd(string id, [FromBody] CreateProductDto dto)
        {
            try
            {
                var updated = _service.UpdateProd(id, dto.Name, dto.Description);

                var response = new ProductResponseDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Description = updated.Description,
                    Price = updated.Price,
                    Quantity = updated.Quantity,
                    Active = updated.Active
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Atualizar preço
       
        [HttpPatch("{id}/price")]
        public ActionResult<ProductResponseDto> UpdatePrice(string id, [FromBody] decimal price)
        {
            try
            {
                var updated = _service.UpdatePrice(id, price);

                var response = new ProductResponseDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Description = updated.Description,
                    Price = updated.Price,
                    Quantity = updated.Quantity,
                    Active = updated.Active
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Atualizar quantidade
        
        [HttpPatch("{id}/quantity")]
        public ActionResult<ProductResponseDto> UpdateQuantity(string id, [FromBody] int quantity)
        {
            try
            {
                var updated = _service.UpdateQuantity(id, quantity);

                var response = new ProductResponseDto
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Description = updated.Description,
                    Price = updated.Price,
                    Quantity = updated.Quantity,
                    Active = updated.Active
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        // Inativar produto
       
        [HttpPatch("{id}/inactivate")]
        public IActionResult Inactivate(string id)
        {
            try
            {
                _service.Inactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    
        // Deletar produto
        
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _service.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

//DTOs apenas na borda da API (entrada e saída).
// Controller converte DTO ↔ Domain e vice-versa.
// DTO na Controller: obrigatório, porque a Controller é a borda que fala com o cliente.