namespace Ecommerce.API.DTO.Sale
{
    public class SaleRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; } = DateTime.Now;

    }
}
