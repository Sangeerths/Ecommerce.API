using Ecommerce.API.Data;
using Ecommerce.API.DTO.Pagination;
using Ecommerce.API.DTO.Sale;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public interface ISaleService
    {
        Task<PagedResponse<SaleResponseDto>> GetAllSalesAsync(PaginationParams paginationParams);
        Task<SaleResponseDto> GetSaleByIdAsync(int saleId);
        Task<SaleResponseDto> CreateSaleAsync(SaleRequestDto saleDto);
        Task<SaleResponseDto> UpdateSaleAsync(int saleId, SaleRequestDto saleDto);
        Task<bool> DeleteSaleAsync(int saleId);
    }
    public class SaleService : ISaleService
    {
        private readonly EcommerceDbContext _dbContext;

        public SaleService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<PagedResponse<SaleResponseDto>> GetAllSalesAsync(
            PaginationParams paginationParams)
        {
            var query = _dbContext.Sales
                .Where(s => !s.IsDeleted);

            var totalRecords = await query.CountAsync();

            var sales = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(s => new SaleResponseDto
                {
                    SaleId = s.SaleId,
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    TotalPrice = s.TotalAmount,
                    SaleDate = s.SaleDate,
                    CustomerName = s.CustomerName
                })
                .ToListAsync();

            return new PagedResponse<SaleResponseDto>(
                sales,
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalRecords);
        }


        public async Task<SaleResponseDto> GetSaleByIdAsync(int saleId)
        {
            var sale = await _dbContext.Sales.FindAsync(saleId);
            if (sale == null)
            {
                return null;
            }
            return new SaleResponseDto
            {
                SaleId = sale.SaleId,
                ProductId = sale.ProductId,
                Quantity = sale.Quantity,
                TotalPrice = sale.TotalAmount,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName
            };
        }

        public async Task<SaleResponseDto> CreateSaleAsync(SaleRequestDto saleDto)
        {
            var sale = new Models.Sale
            {
                ProductId = saleDto.ProductId,
                Quantity = saleDto.Quantity,
                UnitPrice = saleDto.UnitPrice,
                CustomerName = saleDto.CustomerName,
                SaleDate = DateTime.Now
            };
            _dbContext.Sales.Add(sale);
            await _dbContext.SaveChangesAsync();
            return new SaleResponseDto
            {
                SaleId = sale.SaleId,
                ProductId = sale.ProductId,
                Quantity = sale.Quantity,
                TotalPrice = sale.TotalAmount,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName
            };
        }

        public async Task<SaleResponseDto> UpdateSaleAsync(int saleId, SaleRequestDto saleDto)
        {
            var sale = await _dbContext.Sales.FindAsync(saleId);
            if (sale == null)
            {
                return null;
            }
            sale.ProductId = saleDto.ProductId;
            sale.Quantity = saleDto.Quantity;
            sale.UnitPrice = saleDto.UnitPrice;
            sale.CustomerName = saleDto.CustomerName;
            sale.SaleDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return new SaleResponseDto
            {
                SaleId = sale.SaleId,
                ProductId = sale.ProductId,
                Quantity = sale.Quantity,
                TotalPrice = sale.TotalAmount,
                SaleDate = sale.SaleDate,
                CustomerName = sale.CustomerName
            };
        }

        public async Task<bool> DeleteSaleAsync(int saleId)
        {
            var sale = await _dbContext.Sales.FindAsync(saleId);
            if (sale == null)
            {
                return false;
            }
            sale.IsDeleted = true;
            sale.DeletedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
