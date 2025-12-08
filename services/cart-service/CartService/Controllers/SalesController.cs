using Microsoft.AspNetCore.Mvc;
using CartService.Service;
using CartService.DTO.Requests;
using CartService.DTO.Responses;
using CartService.Domain;
using CartService.Logging;
using System.Diagnostics;

namespace CartService.Controllers
{
    [ApiController]
    [Route("sales")]
    public class SaleController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly LoggerService _logger;

        public SaleController(ISalesService salesService, LoggerService logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        //  Criar uma venda
        [HttpPost]
        public IActionResult CreateSale([FromBody] SaleRequestDTO dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<SaleResponseDTO>();

            try
            {
                _logger.Log($"[CreateSale] Iniciando criação da venda para cliente: {dto.ClientId}");
                var sale = _salesService.CreateSale(dto.ClientId);

                response.Data = new SaleResponseDTO
                {
                    Id = sale.Id,
                    ClientId = sale.ClientId,
                    Status = sale.Status,
                    CreatedAt = sale.CreatedAt,
                    UpdatedAt = sale.UpdatedAt,
                    Items = new List<SaleItemResponseDTO>()
                };

                response.Message = "Venda criada com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao criar venda";
                response.Error = ex.Message;
                _logger.Log($"[CreateSale][ERRO] {ex.Message}");
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

        //  Adicionar item à venda
        [HttpPost("{saleId}/items")]
        public IActionResult AddItem(string saleId, [FromBody] SaleItemRequestDTO dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.Log($"[AddItem] Adicionando item {dto.ProductId} à venda {saleId}");
                _salesService.AddItem(saleId, dto.ProductId, dto.Quantity);

                response.Message = "Item adicionado com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao adicionar item";
                response.Error = ex.Message;
                _logger.Log($"[AddItem][ERRO] {ex.Message}");
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

        //  Atualizar quantidade de item
        [HttpPut("items/{itemId}")]
        public IActionResult UpdateItemQuantity(string itemId, [FromBody] SaleItemRequestDTO dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.Log($"[UpdateItemQuantity] Atualizando item {itemId} para {dto.Quantity}");
                _salesService.UpdateItemQuantity(itemId, dto.Quantity);

                response.Message = "Quantidade atualizada com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao atualizar item";
                response.Error = ex.Message;
                _logger.Log($"[UpdateItemQuantity][ERRO] {ex.Message}");
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

        //  Cancelar venda
        [HttpPut("{saleId}/cancel")]
        public IActionResult CancelSale(string saleId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<object>();

            try
            {
                _logger.Log($"[CancelSale] Cancelando venda {saleId}");
                _salesService.CancelSale(saleId);
                response.Message = "Venda cancelada com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao cancelar venda";
                response.Error = ex.Message;
                _logger.Log($"[CancelSale][ERRO] {ex.Message}");
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

        //  Consultar venda por ID
        [HttpGet("{saleId}")]
        public IActionResult GetSaleById(string saleId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<SaleResponseDTO>();

            try
            {
                _logger.Log($"[GetSaleById] Consultando venda {saleId}");
                var sale = _salesService.GetSaleById(saleId);

                if (sale == null)
                {
                    response.Message = "Venda não encontrada";
                    response.Error = "NotFound";
                    return NotFound(response);
                }

                response.Data = new SaleResponseDTO
                {
                    Id = sale.Id,
                    ClientId = sale.ClientId,
                    Status = sale.Status,
                    CreatedAt = sale.CreatedAt,
                    UpdatedAt = sale.UpdatedAt,
                    Items = sale.Items.Select(i => new SaleItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt
                    }).ToList()
                };

                response.Message = "Venda encontrada com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar venda";
                response.Error = ex.Message;
                _logger.Log($"[GetSaleById][ERRO] {ex.Message}");
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

        //  Consultar vendas por produto
        [HttpGet("product/{productId}")]
        public IActionResult GetSalesByProduct(string productId)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<List<SaleResponseDTO>>();

            try
            {
                _logger.Log($"[GetSalesByProduct] Consultando vendas do produto {productId}");
                var sales = _salesService.GetSalesByProduct(productId);

                response.Data = sales.Select(s => new SaleResponseDTO
                {
                    Id = s.Id,
                    ClientId = s.ClientId,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    Items = s.Items.Select(i => new SaleItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt
                    }).ToList()
                }).ToList();

                response.Message = "Vendas do produto recuperadas com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar vendas do produto";
                response.Error = ex.Message;
                _logger.Log($"[GetSalesByProduct][ERRO] {ex.Message}");
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

        // Consultar vendas por status
        [HttpGet("status/{status}")]
        public IActionResult GetSalesByStatus(int status)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ApiResponse<List<SaleResponseDTO>>();

            try
            {
                _logger.Log($"[GetSalesByStatus] Consultando vendas com status {status}");
                var sales = _salesService.GetSalesByStatus(status);

                response.Data = sales.Select(s => new SaleResponseDTO
                {
                    Id = s.Id,
                    ClientId = s.ClientId,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    Items = s.Items.Select(i => new SaleItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt
                    }).ToList()
                }).ToList();

                response.Message = "Vendas com status recuperadas com sucesso";
            }
            catch (Exception ex)
            {
                response.Message = "Erro ao consultar vendas por status";
                response.Error = ex.Message;
                _logger.Log($"[GetSalesByStatus][ERRO] {ex.Message}");
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
