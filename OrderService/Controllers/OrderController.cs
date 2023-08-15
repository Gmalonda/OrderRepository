using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _collection;
        public OrderController()
        {

            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("orderdb"); 
            _collection = database.GetCollection<Order>("order");
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                var product = new Product { ProductId = orderDto.ProductId, ProductName = orderDto.ProductName };
                var client = new Client { ClientAddress = orderDto.ClientAddress, ClientId = orderDto.ClientId, ClientName = orderDto.ClientName };
                var order = new Order
                {
                    IsProcesing = false,
                    Quantity = orderDto.Quantity,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Envoyé",
                    Price= orderDto.Price,
                    TotalPrice = orderDto.Price * orderDto.Quantity,
                    OrderProduct = product,
                    OrderClient = client
                };

                await  _collection.InsertOneAsync(order);

                var latestDocument = await _collection
                .Find(Builders<Order>.Filter.Empty) 
                .SortByDescending(p => p.OrderId)
                .Limit(1)
                .FirstOrDefaultAsync();



                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(latestDocument.OrderId) ;
                    channel.BasicPublish(exchange: "", routingKey: "order_queue", basicProperties: null, body: body);

                }

                return Ok($"Commande envoyée : Commande n° {latestDocument.OrderId}");
            }

            catch (Exception e)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
            }
        }
    }
}
