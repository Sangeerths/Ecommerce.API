using Ecommerce.API.DTO.Pagination;
using Ecommerce.API.DTO.Sale;
using Ecommerce.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSales([FromQuery] PaginationParams paginationParams)
        {
            var sales = await _saleService.GetAllSalesAsync(paginationParams);
            return Ok(sales);
        }
        [HttpGet("{saleId}")]
        public async Task<IActionResult> GetSaleById(int saleId)
        {
            var sale = await _saleService.GetSaleByIdAsync(saleId);
            if (sale == null)
            {
                return NotFound();
            }
            return Ok(sale);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleRequestDto saleDto)
        {
            var createdSale = await _saleService.CreateSaleAsync(saleDto);
            return CreatedAtAction(nameof(GetSaleById), new { saleId = createdSale.SaleId }, createdSale);
        }
        [HttpPut("{saleId}")]
        public async Task<IActionResult> UpdateSale(int saleId, [FromBody] SaleRequestDto saleDto)
        {
            var updatedSale = await _saleService.UpdateSaleAsync(saleId, saleDto);
            if (updatedSale == null)
            {
                return NotFound();
            }
            return Ok(updatedSale);
        }
        [HttpDelete("{saleId}")]
        public async Task<IActionResult> DeleteSale(int saleId)
        {
            var deleted = await _saleService.DeleteSaleAsync(saleId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
