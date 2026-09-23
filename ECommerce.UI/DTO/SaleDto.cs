namespace Ecommerce.UI.DTO.Sale;

public class SaleDto
{
    public int SaleId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal TotalPrice { get; set; }

    public List<SaleItemDto> Items { get; set; } = new();
}
public class SaleItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}
