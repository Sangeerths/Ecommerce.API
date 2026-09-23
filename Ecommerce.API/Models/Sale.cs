namespace Ecommerce.API.Models;

public class Sale
{
    public int SaleId { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    public string CustomerName { get; set; } = string.Empty;

    public ICollection<SaleItem> SaleItems { get; set; }
        = new List<SaleItem>();

    public decimal TotalAmount =>
        SaleItems.Sum(x => x.UnitPrice * x.Quantity);

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }
}