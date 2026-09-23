namespace Ecommerce.API.DTO.Sale;

public class SaleRequestDto
{
    public string CustomerName { get; set; } = string.Empty;
    public List<SaleItemRequestDto> Items { get; set; } = new();
}

public class SaleItemRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
