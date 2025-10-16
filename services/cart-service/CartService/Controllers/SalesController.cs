using Microsoft.AspNetCore.Mvc;
using CartService.Service;
using CartService.DTO.Requests;
using CartService.DTO.Responses;
using CartService.Domain;
using CartService.Logging;
using System.Collections.Generic;

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

        // Criar uma venda
        [HttpPost]
        public IActionResult CreateSale([FromBody] SaleRequestDTO dto)
        {
            _logger.Log($"[CreateSale] Iniciando criação da venda para cliente: {dto.ClientId}");
            var sale = _salesService.CreateSale(dto.ClientId);
            _logger.Log($"[CreateSale] Venda criada: {sale.Id} para cliente {sale.ClientId}");

            var response = new SaleResponseDTO
            {
                Id = sale.Id,
                ClientId = sale.ClientId,
                Status = sale.Status,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt,
                Items = new List<SaleItemResponseDTO>()
            };

            return Ok(new ApiResponse<SaleResponseDTO>
            {
                Message = "Venda criada com sucesso",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = response
            });
        }

        // Adicionar item à venda
        [HttpPost("{saleId}/items")]
        public IActionResult AddItem(string saleId, [FromBody] SaleItemRequestDTO dto)
        {
            _logger.Log($"[AddItem] Adicionando item {dto.ProductId} (qnt: {dto.Quantity}) à venda {saleId}");
            _salesService.AddItem(saleId, dto.ProductId, dto.Quantity);
            _logger.Log($"[AddItem] Item adicionado com sucesso à venda {saleId}");

            return Ok(new ApiResponse<object>
            {
                Message = "Item adicionado com sucesso",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = null
            });
        }

        // Alterar quantidade de item
        [HttpPut("items/{itemId}")]
        public IActionResult UpdateItemQuantity(string itemId, [FromBody] SaleItemRequestDTO dto)
        {
            _logger.Log($"[UpdateItemQuantity] Atualizando quantidade do item {itemId} para {dto.Quantity}");
            _salesService.UpdateItemQuantity(itemId, dto.Quantity);
            _logger.Log($"[UpdateItemQuantity] Quantidade do item {itemId} atualizada com sucesso");

            return Ok(new ApiResponse<object>
            {
                Message = "Quantidade atualizada com sucesso",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = null
            });
        }

        // Cancelar venda
        [HttpPut("{saleId}/cancel")]
        public IActionResult CancelSale(string saleId)
        {
            _logger.Log($"[CancelSale] Cancelando venda {saleId}");
            _salesService.CancelSale(saleId);
            _logger.Log($"[CancelSale] Venda {saleId} cancelada com sucesso");

            return Ok(new ApiResponse<object>
            {
                Message = "Venda cancelada com sucesso",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = null
            });
        }

        // Consultar venda por ID
        [HttpGet("{saleId}")]
        public IActionResult GetSaleById(string saleId)
        {
            _logger.Log($"[GetSaleById] Consultando venda {saleId}");
            var sale = _salesService.GetSaleById(saleId);

            if (sale == null)
            {
                _logger.Log($"[GetSaleById] Venda {saleId} não encontrada");
                return NotFound(new ApiResponse<object>
                {
                    Message = "Venda não encontrada",
                    Timestamp = DateTime.UtcNow,
                    Elapsed = 0,
                    Error = "NotFound",
                    Data = null
                });
            }

            var response = new SaleResponseDTO
            {
                Id = sale.Id,
                ClientId = sale.ClientId,
                Status = sale.Status,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt,
                Items = sale.Items.ConvertAll(i => new SaleItemResponseDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
            };

            _logger.Log($"[GetSaleById] Venda {saleId} encontrada com {sale.Items.Count} itens");

            return Ok(new ApiResponse<SaleResponseDTO>
            {
                Message = "Venda encontrada",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = response
            });
        }

        // Consultar vendas por produto
        [HttpGet("product/{productId}")]
        public IActionResult GetSalesByProduct(string productId)
        {
            _logger.Log($"[GetSalesByProduct] Consultando vendas do produto {productId}");
            var sales = _salesService.GetSalesByProduct(productId);

            _logger.Log($"[GetSalesByProduct] {sales.Count} vendas encontradas para o produto {productId}");

            var responseList = sales.ConvertAll(s => new SaleResponseDTO
            {
                Id = s.Id,
                ClientId = s.ClientId,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Items = s.Items.ConvertAll(i => new SaleItemResponseDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
            });

            return Ok(new ApiResponse<List<SaleResponseDTO>>
            {
                Message = "Vendas retornadas",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = responseList
            });
        }

        // Consultar vendas por status
        [HttpGet("status/{status}")]
        public IActionResult GetSalesByStatus(int status)
        {
            _logger.Log($"[GetSalesByStatus] Consultando vendas com status {status}");
            var sales = _salesService.GetSalesByStatus(status);

            _logger.Log($"[GetSalesByStatus] {sales.Count} vendas encontradas com status {status}");

            var responseList = sales.ConvertAll(s => new SaleResponseDTO
            {
                Id = s.Id,
                ClientId = s.ClientId,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Items = s.Items.ConvertAll(i => new SaleItemResponseDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
            });

            return Ok(new ApiResponse<List<SaleResponseDTO>>
            {
                Message = "Vendas retornadas",
                Timestamp = DateTime.UtcNow,
                Elapsed = 0,
                Error = "",
                Data = responseList
            });
        }
    }
}
