namespace OrderService
{
    public class OrderSolr
    {
        public string? OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderDate { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public string? Quantity { get; set; }
        public string? Price { get; set; }
        public string? TotalPrice { get; set; }
        public string? IsProcesing { get; set; }

    }
}
