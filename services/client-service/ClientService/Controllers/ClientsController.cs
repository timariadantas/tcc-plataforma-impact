using ClientService.Domain;
using ClientService.Service;
using ClientService.DTO.Requests;
using ClientService.DTO.Responses;
using ClientService.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using ClientService.Domain.Exceptions;
using ClientService.Mappers;

namespace ClientService.Controllers
{
    [ApiController]
    [Route("clients")]
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
        public IActionResult Create([FromBody] CreateClientRequestDto dto)
        {
            var response = new ApiResponse<ClientResponseDto>();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var client = _service.Create(dto.Name, dto.Surname, dto.Email, dto.Birthdate);

                response.Data = ClientMapper.ToResponse(client);
                response.Message = "Cliente criado com sucesso";

                _logger.Log($"Cliente criado: {client.Id} - {client.Name} {client.Surname}", Logging.LogLevel.INFO);

                return Ok(response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = System.DateTime.UtcNow;
            }
        }

       
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var response = new ApiResponse<ClientResponseDto>();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var client = _service.GetById(id);
                if (client == null)
                    throw new ClientNotFoundException(id);

                response.Data = ClientMapper.ToResponse(client);
                response.Message = "Cliente consultado com sucesso";

                _logger.Log($"Cliente consultado: {client.Id}", Logging.LogLevel.INFO);

                return Ok(response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = System.DateTime.UtcNow;
            }
        }

       
        [HttpGet]
        public IActionResult GetAll()
        {
            var response = new ApiResponse<List<ClientResponseDto>>();
            var stopwatch = Stopwatch.StartNew();

            var clients = _service.GetAll();
            response.Data = clients.Select(ClientMapper.ToResponse).ToList();
            response.Message = "Lista de clientes recuperada com sucesso";

            _logger.Log($"Clientes recuperados: {clients.Count}", Logging.LogLevel.INFO);

            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = System.DateTime.UtcNow;

            return Ok(response);
        }

       
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UpdateClientRequestDto dto)
        {
            var response = new ApiResponse<ClientResponseDto>();
            var stopwatch = Stopwatch.StartNew();
            
            _service.Update( dto.Name, dto.Surname, dto.Email, dto.Birthdate);
            var client = _service.GetById(id);

            response.Data =  ClientMapper.ToResponse(client!);
            response.Message = "Cliente atualizado com sucesso";

            _logger.Log($"Cliente atualizado: ", Logging.LogLevel.INFO);

            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = System.DateTime.UtcNow;

            return Ok(response);
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var response = new ApiResponse<DeleteResponseDto>();
            var stopwatch = Stopwatch.StartNew();

            _service.Delete(id);

            response.Data = new DeleteResponseDto { Message = $"Cliente deletado com sucesso: {id}" };
            response.Message = "Cliente deletado com sucesso";

            _logger.Log($"Cliente deletado: {id}", Logging.LogLevel.INFO);

            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = System.DateTime.UtcNow;

            return Ok(response);
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(string id)
        {
            var response = new ApiResponse<ClientResponseDto>();
            var stopwatch = Stopwatch.StartNew();

            _service.Deactivate(id);

            var client = _service.GetById(id);
            response.Data = ClientMapper.ToResponse(client!);
            response.Message = "Cliente desativado com sucesso";

            _logger.Log($"Cliente desativado: {id}", Logging.LogLevel.INFO);

            stopwatch.Stop();
            response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
            response.Timestamp = System.DateTime.UtcNow;

            return Ok(response);
        }
    }
}
