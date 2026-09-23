namespace Ecommerce.API.Models;

public class SaleItem
{
    public int SaleItemId { get; set; }

    public int SaleId { get; set; }

    public Sale Sale { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount =>
        UnitPrice * Quantity;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}