using Ecommerce.API.Data;
using Ecommerce.API.DTO.Pagination;
using Ecommerce.API.DTO.Product;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.API.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponseDto>> GetAllProductsAsync(PaginationParams paginationParams);
    Task<ProductResponseDto> GetProductByIdAsync(int productId);
    Task<ProductResponseDto> CreateProductAsync(ProductRequestDto productDto);
    Task<ProductResponseDto> UpdateProductAsync(int productId, ProductRequestDto productDto);
    Task<bool> DeleteProductAsync(int productId);
}
public class ProductService : IProductService
{
    private readonly EcommerceDbContext _dbContext;

    public ProductService(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<ProductResponseDto>> GetAllProductsAsync(
       PaginationParams paginationParams)
    {
        var query = _dbContext.Products
            .Where(p => !p.IsDeleted);

        var totalRecords = await query.CountAsync();

        var products = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        return new PagedResponse<ProductResponseDto>(
            products,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            totalRecords);
    }


    public async Task<ProductResponseDto> GetProductByIdAsync(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        if (product == null)
        {
            return null;
        }
        return new ProductResponseDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId
        };
    }
    
    public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto productDto)
    {
        var product = new Models.Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = (decimal)productDto.Price,
            CategoryId = (int)productDto.CategoryId,
            StockQuantity = (int)productDto.StockQuantity,
        };
        var result = await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();
        return new ProductResponseDto
        {
            ProductId = result.Entity.ProductId,
            Name = result.Entity.Name,
            Description = result.Entity.Description,
            Price = result.Entity.Price,
            StockQuantity = result.Entity.StockQuantity,
            CategoryId = result.Entity.CategoryId
        };
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int productId, ProductRequestDto productDto)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        if (product == null)
        {
            return null;
        }
        if (!string.IsNullOrWhiteSpace(productDto.Name))
        {
            product.Name = productDto.Name;
        }
        if (!string.IsNullOrWhiteSpace(productDto.Description))
        {
            product.Description = productDto.Description;
        }
        if (productDto.Price.HasValue)
        {
            product.Price = productDto.Price.Value;
        }
        if (productDto.StockQuantity.HasValue)
        {
            product.StockQuantity = productDto.StockQuantity.Value;
        }
        if (productDto.CategoryId.HasValue)
        {
            product.CategoryId = productDto.CategoryId.Value;
        }
        await _dbContext.SaveChangesAsync();
        return new ProductResponseDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
    }
    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        if (product == null)
        {
            return false;
        }
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
