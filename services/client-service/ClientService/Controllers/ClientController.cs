using ClientService.Domain;
using ClientService.Service;
using Microsoft.AspNetCore.Mvc;
using ClientService.Logging;

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

        // POST api/client
        [HttpPost]
        public IActionResult Create(Client client)
        {
            try
            {
                _service.Create(client); // método síncrono no Service
                _logger.Log($"Cliente criado: {client.Name}, ID: {client.Id}");
                return Ok(client);
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao criar cliente: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/client/{id}
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

                _logger.Log($"Cliente consultado:  ID: {client.Id}");
                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao consultar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        // GET api/client
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var clients = _service.GetAll();
                _logger.Log($"Consulta de todos os clientes realizada. Total: {clients.Count}");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao consultar todos os clientes: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        // PUT api/client/{id}
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] Client client)
        {
            try
            {
                 client.Id = id;
                _service.Update(client);
                _logger.Log($"Cliente atualizado: {client.Name}, ID: {id}");
                return Ok(client);
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao atualizar cliente ID {id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }

        // DELETE api/client/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                _service.Delete(id);
                _logger.Log($"Cliente deletado: ID {id}");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao deletar cliente ID {id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao deletar cliente ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno" });
            }
        }
    }
}
