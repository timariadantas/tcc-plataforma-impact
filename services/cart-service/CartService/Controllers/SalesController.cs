using Microsoft.AspNetCore.Mvc;
using CartService.Domain;

using CartService.Service;
using CartService.DTO.Requests;
using CartService.DTO.Responses;
using CartService.Logging;
using CartService.Mapper;
using System.Diagnostics;
using System.Linq;
using CartService.Domain.Exceptions;

namespace CartService.Controllers
{
    [ApiController]
    [Route("sales")]
    public class SaleController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILoggerService _logger;

        public SaleController(ISalesService salesService, ILoggerService logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        // Criar uma venda
        [HttpPost]
        public IActionResult CreateSale([FromBody] SaleRequestDTO request)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<SaleResponseDTO>();

            try
            {
                _logger.LogInformation("[CreateSale] Iniciando criação da venda para cliente: {0}", request.ClientId);

        // Converte os DTOs de items para domain
        List<SaleItem>? items = null;
        if (request.Items != null && request.Items.Count > 0)
        {
            items = request.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();
        }
                var sale = _salesService.CreateSale(request.ClientId, items);

                response.Data = SaleMapper.ToResponse(sale);
                response.Message = "Venda criada com sucesso";

                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[CreateSale][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[CreateSale][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[CreateSale][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao criar venda";
                response.Error = ex.Message;
                _logger.LogError(ex, "[CreateSale][500] Erro inesperado criando venda for client {0}", request.ClientId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Adicionar item à venda
        [HttpPost("{saleId}/items")]
        public IActionResult AddItem(string saleId, [FromBody] SaleItemRequestDTO dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.LogInformation("[AddItem] Adicionando item {0} à venda {1} (qtd {2})", dto.ProductId, saleId, dto.Quantity);
                _salesService.AddItem(saleId, dto.ProductId, dto.Quantity);

                response.Message = "Item adicionado com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[AddItem][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[AddItem][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[AddItem][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao adicionar item";
                response.Error = ex.Message;
                _logger.LogError(ex, "[AddItem][500] Erro adicionando item {0} na venda {1}", dto.ProductId, saleId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Atualizar quantidade de item
        [HttpPut("items/{itemId}")]
        public IActionResult UpdateItemQuantity(string itemId, [FromBody] SaleItemRequestDTO dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.LogInformation("[UpdateItemQuantity] Atualizando item {0} para {1}", itemId, dto.Quantity);
                _salesService.UpdateItemQuantity(itemId, dto.Quantity);

                response.Message = "Quantidade atualizada com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[UpdateItemQuantity][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[UpdateItemQuantity][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[UpdateItemQuantity][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao atualizar item";
                response.Error = ex.Message;
                _logger.LogError(ex, "[UpdateItemQuantity][500] Erro atualizando item {0}", itemId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Cancelar venda
        [HttpPut("{saleId}/cancel")]
        public IActionResult CancelSale(string saleId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.LogInformation("[CancelSale] Cancelando venda {0}", saleId);
                _salesService.CancelSale(saleId);

                response.Message = "Venda cancelada com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[CancelSale][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[CancelSale][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[CancelSale][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao cancelar venda";
                response.Error = ex.Message;
                _logger.LogError(ex, "[CancelSale][500] Erro cancelando venda {0}", saleId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Consultar venda por ID
        [HttpGet("{saleId}")]
        public IActionResult GetSaleById(string saleId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<SaleResponseDTO>();

            try
            {
                _logger.LogInformation("[GetSaleById] Consultando venda {0}", saleId);
                var sale = _salesService.GetSaleById(saleId);

                if (sale == null)
                {
                    response.Message = "Venda não encontrada";
                    response.Error = "NotFound";
                    _logger.LogWarning("[GetSaleById][404] Venda não encontrada: {0}", saleId);
                    return NotFound(response);
                }

                response.Data = SaleMapper.ToResponse(sale);
                response.Message = "Venda encontrada com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[GetSaleById][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[GetSaleById][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[GetSaleById][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar venda";
                response.Error = ex.Message;
                _logger.LogError(ex, "[GetSaleById][500] Erro consultando venda {0}", saleId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Consultar vendas por produto
        [HttpGet("product/{productId}")]
        public IActionResult GetSalesByProduct(string productId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<List<SaleResponseDTO>>();

            try
            {
                _logger.LogInformation("[GetSalesByProduct] Consultando vendas do produto {0}", productId);
                var sales = _salesService.GetSalesByProduct(productId);

                response.Data = sales.Select(SaleMapper.ToResponse).ToList();
                response.Message = "Vendas do produto recuperadas com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[GetSalesByProduct][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[GetSalesByProduct][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[GetSalesByProduct][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar vendas do produto";
                response.Error = ex.Message;
                _logger.LogError(ex, "[GetSalesByProduct][500] Erro consultando vendas para product {0}", productId);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }

        // Consultar vendas por status
        [HttpGet("status/{status}")]
        public IActionResult GetSalesByStatus(int status)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<List<SaleResponseDTO>>();

            try
            {
                _logger.LogInformation("[GetSalesByStatus] Consultando vendas com status {0}", status);
                var sales = _salesService.GetSalesByStatus(status);

                response.Data = sales.Select(SaleMapper.ToResponse).ToList();
                response.Message = "Vendas com status recuperadas com sucesso";
                return Ok(response);
            }
            catch (DomainValidationException ex)
            {
                response.Message = ex.Message;
                response.Error = "ValidationError";
                _logger.LogWarning("[GetSalesByStatus][400] {0}", ex.Message);
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response.Message = ex.Message;
                response.Error = "BusinessRule";
                _logger.LogWarning("[GetSalesByStatus][422] {0}", ex.Message);
                return StatusCode(422, response);
            }
            catch (NotFoundException ex)
            {
                response.Message = ex.Message;
                response.Error = "NotFound";
                _logger.LogWarning("[GetSalesByStatus][404] {0}", ex.Message);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar vendas por status";
                response.Error = ex.Message;
                _logger.LogError(ex, "[GetSalesByStatus][500] Erro consultando vendas por status {0}", status);
                return StatusCode(500, response);
            }
            finally
            {
                stopwatch.Stop();
                response.Elapsed = (int)stopwatch.ElapsedMilliseconds;
                response.Timestamp = DateTime.UtcNow;
            }
        }
    }
}
// O Controller deve NUNCA tocar no Id.