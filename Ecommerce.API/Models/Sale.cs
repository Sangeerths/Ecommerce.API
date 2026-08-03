namespace Ecommerce.API.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }   
        public int ProductId { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public decimal TotalAmount => UnitPrice * Quantity;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

    }
}
