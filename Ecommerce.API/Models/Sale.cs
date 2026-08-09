namespace Ecommerce.API.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public int Quantity { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public string CustomerName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public decimal TotalAmount => UnitPrice * Quantity;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}