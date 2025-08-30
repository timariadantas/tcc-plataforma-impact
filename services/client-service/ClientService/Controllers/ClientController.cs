using ClientService.Domain;
using ClientService.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClientService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _service;
        public ClientController(IClientService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create(Client client)
        {
            try
            {
                _service.Create(client);
                return Ok(client);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var client = _service.GetById(id);
            if (client == null) return NotFound();
            return Ok(client);

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _service.Delete(id);
            return NoContent();
        }

    }
}

