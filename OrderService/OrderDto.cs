namespace OrderService
{
    public class OrderDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public int? Price { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
    }
}
