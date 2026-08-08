namespace Ecommerce.API.DTO.Sale
{
    public class SaleResponseDto
    {
        public int SaleId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public int ProductId { get; set; }
    }
}
