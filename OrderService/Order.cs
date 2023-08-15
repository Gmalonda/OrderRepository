using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public int? Price { get; set; }
        public int? TotalPrice { get; set; }
        public bool? IsProcesing { get; set; }
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
    }
}
