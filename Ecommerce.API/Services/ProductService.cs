using Ecommerce.API.Data;
using Ecommerce.API.DTO.Product;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();   
    }
    public class ProductService : IProductService
    {
        private readonly EcommerceDbContext _dbContext;

        public ProductService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _dbContext.Products.ToListAsync();
            return products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId
            }).ToList();
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
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
    }
}
