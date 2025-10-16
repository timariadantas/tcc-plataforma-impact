using ClientService.Domain;
using ClientService.Service;
using ClientService.DTO.Responses;
using ClientService.DTO.Requests;
using ClientService.Logging;
using Microsoft.AspNetCore.Mvc;


namespace ClientService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;
        private readonly LoggerService _logger;

        public ClientController(IClientService service, LoggerService logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateClientDto dto)
        {
            try
            {
                // Converte DTO para Domain
                var client = new Client
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    Birthdate = dto.Birthdate
                };

                _service.Create(client);

                _logger.Log($"Cliente criado: {client.Name}, ID: {client.Id}");

                // Converte Domain para Response DTO
                var response = new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    Active = client.Active
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao criar cliente: {ex.Message}");
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var client = _service.GetById(id);
                if (client == null)
                {
                    _logger.Log($"Cliente não encontrado: ID {id}");
                    return NotFound(new { message = "Cliente não encontrado" });
                }

                var response = new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    Active = client.Active
                };

                _logger.Log($"Cliente consultado: ID: {client.Id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao consultar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var clients = _service.GetAll();
                _logger.Log($"Clientes recuperados: {clients.Count}");

                var response = clients.Select(client =>
                {
                    _logger.Log($"Mapeando cliente: {client.Id}");
                    return new ClientResponseDto
                    {
                        Id = client.Id,
                        Name = client.Name,
                        Surname = client.Surname,
                        Email = client.Email,
                        Birthdate = client.Birthdate,
                        Active = client.Active
                    };
                }).ToList();

                _logger.Log("Mapeamento completo de todos os clientes.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro no GetAll: {ex.GetType().Name} - {ex.Message}");
                _logger.Log(ex.StackTrace ?? "Sem stack trace");
                 return StatusCode(500, new ErrorResponse { Message = "Erro interno" });
            }
            }
        
    
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UpdateClientDto dto)
        {
            try
            {
                var client = new Client
                {
                    Id = id,
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    Birthdate = dto.Birthdate,
                    Active = dto.Active
                };

                _service.Update(client);

                var response = new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    CreatedAt = client.CreatedAt,
                    UpdatedAt = client.UpdatedAt,
                    Active = client.Active
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao atualizar cliente: {ex.Message}");
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new ErrorResponse { Message = "Erro interno" });
            }
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(string id)
        {
            try
            {
                _service.Delete(id); // aqui deve lançar exceção se não existe

                _logger.Log($"Cliente deletado: ID {id}");
                return Ok(new DeleteResponseDto { Message = $"Cliente deletado com sucesso: ID {id}" });
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao deletar cliente ID {id}: {ex.Message}");
                return BadRequest(new ErrorResponse { Message = ex.Message }); // <- usa ErrorResponse
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao deletar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new ErrorResponse { Message = "Erro interno" }); // usa ErrorResponse
            }
        }

    }
}
