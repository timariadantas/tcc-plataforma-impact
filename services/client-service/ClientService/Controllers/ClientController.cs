using ClientService.Domain;
using ClientService.Service;
using ClientService.DTO.Responses;
using ClientService.DTO.Requests;
using ClientService.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

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
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<ClientResponseDto>();

            try
            {
                var client = new Client
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    Birthdate = dto.Birthdate
                };

                _service.Create(client);

                _logger.Log($"Cliente criado: {client.Name}, ID: {client.Id}", Logging.LogLevel.INFO);

                response.Data = new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    Active = client.Active
                };
                response.Message = "Cliente criado com sucesso";
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao criar cliente: {ex.Message}", Logging.LogLevel.WARNING);
                response.Error = ex.Message;
                response.Message = "Erro ao criar cliente";
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao criar cliente: {ex.Message}", Logging.LogLevel.ERROR);
                response.Error = ex.Message;
                response.Message = "Erro interno ao criar cliente";
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }

            return response.Error == string.Empty ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<ClientResponseDto>();

            try
            {
                var client = _service.GetById(id);
                if (client == null)
                {
                    _logger.Log($"Cliente não encontrado: ID {id}", Logging.LogLevel.WARNING);
                    response.Message = "Cliente não encontrado";
                    response.Error = "Cliente não existe";
                    return NotFound(response);
                }

                response.Data = new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    Active = client.Active
                };
                response.Message = "Cliente consultado com sucesso";

                _logger.Log($"Cliente consultado: ID {client.Id}", Logging.LogLevel.INFO);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao consultar cliente ID {id}: {ex.Message}", Logging.LogLevel.ERROR);
                response.Error = ex.Message;
                response.Message = "Erro interno";
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
            var response = new ApiResponse<List<ClientResponseDto>>();

            try
            {
                var clients = _service.GetAll();
                _logger.Log($"Clientes recuperados: {clients.Count}", Logging.LogLevel.INFO);

                response.Data = clients.Select(client => new ClientResponseDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Surname = client.Surname,
                    Email = client.Email,
                    Birthdate = client.Birthdate,
                    Active = client.Active
                }).ToList();

                response.Message = "Lista de clientes recuperada com sucesso";
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro no GetAll: {ex.GetType().Name} - {ex.Message}", Logging.LogLevel.ERROR);
                _logger.Log(ex.StackTrace ?? "Sem stack trace", Logging.LogLevel.DEBUG);
                response.Error = ex.Message;
                response.Message = "Erro interno ao recuperar clientes";
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

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UpdateClientDto dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<ClientResponseDto>();

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

                response.Data = new ClientResponseDto
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
                response.Message = "Cliente atualizado com sucesso";

                _logger.Log($"Cliente atualizado: ID {client.Id}", Logging.LogLevel.INFO );
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao atualizar cliente: {ex.Message}", Logging.LogLevel.WARNING);
                response.Error = ex.Message;
                response.Message = "Erro ao atualizar cliente";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao atualizar cliente ID {id}: {ex.Message}", Logging.LogLevel.ERROR);
                response.Error = ex.Message;
                response.Message = "Erro interno ao atualizar cliente";
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

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<DeleteResponseDto>();

            try
            {
                _service.Delete(id);

                response.Data = new DeleteResponseDto
                {
                    Message = $"Cliente deletado com sucesso: ID {id}"
                };
                response.Message = "Cliente deletado com sucesso";

                _logger.Log($"Cliente deletado: ID {id}", Logging.LogLevel.INFO);
            }
            catch (ArgumentException ex)
            {
                _logger.Log($"Falha ao deletar cliente ID {id}: {ex.Message}", Logging.LogLevel.WARNING);
                response.Error = ex.Message;
                response.Message = "Erro ao deletar cliente";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao deletar cliente ID {id}: {ex.Message}", Logging.LogLevel.ERROR);
                response.Error = ex.Message;
                response.Message = "Erro interno ao deletar cliente";
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
