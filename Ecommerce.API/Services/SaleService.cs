using Ecommerce.API.Data;
using Ecommerce.API.DTO.Pagination;
using Ecommerce.API.DTO.Sale;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.API.Services;

public interface ISaleService
{
    Task<PagedResponse<SaleResponseDto>> GetAllSalesAsync(
        PaginationParams paginationParams);

    Task<SaleResponseDto?> GetSaleByIdAsync(int saleId);

    Task<SaleResponseDto> CreateSaleAsync(
        SaleRequestDto saleDto);

    Task<SaleResponseDto?> UpdateSaleAsync(
        int saleId,
        SaleRequestDto saleDto);

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
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .Skip((paginationParams.PageNumber - 1) *
                  paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        var saleDtos = sales.Select(s => new SaleResponseDto
        {
            SaleId = s.SaleId,
            CustomerName = s.CustomerName,
            SaleDate = s.SaleDate,
            TotalPrice = s.SaleItems.Sum(
                si => si.UnitPrice * si.Quantity),

            Items = s.SaleItems.Select(si => new SaleItemResponseDto
            {
                ProductId = si.ProductId,
                ProductName = si.Product.Name,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.UnitPrice * si.Quantity
            }).ToList()
        }).ToList();

        return new PagedResponse<SaleResponseDto>(
            saleDtos,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            totalRecords);
    }

    public async Task<SaleResponseDto?> GetSaleByIdAsync(int saleId)
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s =>
                s.SaleId == saleId &&
                !s.IsDeleted);

        if (sale == null)
        {
            return null;
        }

        return new SaleResponseDto
        {
            SaleId = sale.SaleId,
            CustomerName = sale.CustomerName,
            SaleDate = sale.SaleDate,

            TotalPrice = sale.SaleItems.Sum(
                si => si.UnitPrice * si.Quantity),

            Items = sale.SaleItems.Select(si => new SaleItemResponseDto
            {
                ProductId = si.ProductId,
                ProductName = si.Product.Name,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.UnitPrice * si.Quantity
            }).ToList()
        };
    }

    public async Task<SaleResponseDto> CreateSaleAsync(
        SaleRequestDto saleDto)
    {
        if (saleDto.Items == null || saleDto.Items.Count == 0)
        {
            throw new ArgumentException(
                "A sale must contain at least one product.");
        }

        var sale = new Models.Sale
        {
            CustomerName = saleDto.CustomerName,
            SaleDate = DateTime.UtcNow
        };

        foreach (var itemDto in saleDto.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == itemDto.ProductId &&
                    !p.IsDeleted);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID {itemDto.ProductId} was not found.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Not enough stock for product {product.Name}.");
            }

            var saleItem = new Models.SaleItem
            {
                ProductId = product.ProductId,
                Quantity = itemDto.Quantity,

                // Save the current product price
                // as the historical sale price.
                UnitPrice = product.Price
            };

            sale.SaleItems.Add(saleItem);

            product.StockQuantity -= itemDto.Quantity;
        }

        _dbContext.Sales.Add(sale);

        await _dbContext.SaveChangesAsync();

        return new SaleResponseDto
        {
            SaleId = sale.SaleId,
            CustomerName = sale.CustomerName,
            SaleDate = sale.SaleDate,

            TotalPrice = sale.SaleItems.Sum(
                si => si.UnitPrice * si.Quantity),

            Items = sale.SaleItems.Select(si => new SaleItemResponseDto
            {
                ProductId = si.ProductId,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.UnitPrice * si.Quantity
            }).ToList()
        };
    }

    public async Task<SaleResponseDto?> UpdateSaleAsync(
        int saleId,
        SaleRequestDto saleDto)
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s =>
                s.SaleId == saleId &&
                !s.IsDeleted);

        if (sale == null)
        {
            return null;
        }

        if (saleDto.Items == null || saleDto.Items.Count == 0)
        {
            throw new ArgumentException(
                "A sale must contain at least one product.");
        }

        sale.CustomerName = saleDto.CustomerName;
        sale.UpdatedAt = DateTime.UtcNow;

        // Restore the stock from the old sale.
        foreach (var oldItem in sale.SaleItems)
        {
            var oldProduct = await _dbContext.Products
                .FindAsync(oldItem.ProductId);

            if (oldProduct != null)
            {
                oldProduct.StockQuantity += oldItem.Quantity;
            }
        }

        // Remove old sale items.
        _dbContext.SaleItems.RemoveRange(sale.SaleItems);

        // Add the new sale items.
        foreach (var itemDto in saleDto.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Quantity must be greater than zero.");
            }

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == itemDto.ProductId &&
                    !p.IsDeleted);

            if (product == null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID {itemDto.ProductId} was not found.");
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Not enough stock for product {product.Name}.");
            }

            var saleItem = new Models.SaleItem
            {
                SaleId = sale.SaleId,
                ProductId = product.ProductId,
                Quantity = itemDto.Quantity,

                // Use the current product price for the
                // newly created sale item.
                UnitPrice = product.Price
            };

            sale.SaleItems.Add(saleItem);

            product.StockQuantity -= itemDto.Quantity;
        }

        await _dbContext.SaveChangesAsync();

        return new SaleResponseDto
        {
            SaleId = sale.SaleId,
            CustomerName = sale.CustomerName,
            SaleDate = sale.SaleDate,

            TotalPrice = sale.SaleItems.Sum(
                si => si.UnitPrice * si.Quantity),

            Items = sale.SaleItems.Select(si => new SaleItemResponseDto
            {
                ProductId = si.ProductId,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalPrice = si.UnitPrice * si.Quantity
            }).ToList()
        };
    }

    public async Task<bool> DeleteSaleAsync(int saleId)
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s =>
                s.SaleId == saleId &&
                !s.IsDeleted);

        if (sale == null)
        {
            return false;
        }

        sale.IsDeleted = true;
        sale.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}