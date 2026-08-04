namespace Ecommerce.API.DTO.Category
{
    public class CategoryRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public bool? IsDeleted { get; set; } = false;

    }
}
