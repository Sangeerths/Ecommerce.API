using Ecommerce.API.Data;
using Ecommerce.API.DTO.Category;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto);
    }
    public class CategoryService : ICategoryService
    {
        private readonly EcommerceDbContext _dbContext;

        public CategoryService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList();
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return null;
            }
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto)
        {
            var category = new Models.Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description

            };
            var result = await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return new CategoryResponseDto
            {
                CategoryId = result.Entity.CategoryId,
                Name = result.Entity.Name,
                Description = result.Entity.Description
            };
        }
    }
}
