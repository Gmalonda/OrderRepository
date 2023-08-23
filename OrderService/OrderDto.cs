namespace OrderService
{
    public class OrderDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Quantity { get; set; }
        public string? Price { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
    }
}
