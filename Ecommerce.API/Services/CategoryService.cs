using Ecommerce.API.Data;
using Ecommerce.API.DTO.Category;
using Ecommerce.API.DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    public interface ICategoryService
    {
        Task<PagedResponse<CategoryResponseDto>> GetCategoriesAsync(PaginationParams paginationParams);
        Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
    public class CategoryService : ICategoryService
    {
        private readonly EcommerceDbContext _dbContext;

        public CategoryService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResponse<CategoryResponseDto>> GetCategoriesAsync(
    PaginationParams paginationParams)
        {
            var query = _dbContext.Categories.AsQueryable();

            var totalRecords = await query.CountAsync();

            var categories = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();

            return new PagedResponse<CategoryResponseDto>(
                categories,
                paginationParams.PageNumber,
                paginationParams.PageSize,
                totalRecords);
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

        public async Task<CategoryResponseDto?> UpdateCategoryAsync(
    int categoryId,
    CategoryRequestDto categoryDto)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                category.Name = categoryDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(categoryDto.Description))
            {
                category.Description = categoryDto.Description;
            }

            category.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }


    }
}
