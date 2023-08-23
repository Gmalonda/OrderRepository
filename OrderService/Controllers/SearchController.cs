using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SolrNet;
using SolrNet.Impl;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISolrOperations<OrderSolr> _solrOperations;
        private readonly IMongoCollection<Order> _collection;

        public SearchController(ISolrOperations<OrderSolr> solrOperations)
        {
            _solrOperations = solrOperations;

            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("orderdb");
            _collection = database.GetCollection<Order>("order");
        }
        [HttpPost]

        public async Task<IActionResult> IndexDocumentAsync()
        {
            try
            {
                string solrBaseUrl = "http://localhost:8983/solr/new_core";

                var orders = new List<dynamic>();

                using (HttpClient client = new HttpClient())
                {
                    _collection.Find(o => true).ToList().ForEach( o =>
                   {
                       var oTransform = new
                       {
                        o.OrderId,
                        o.OrderStatus,
                        o.OrderDate,
                        o.OrderProduct?.ProductId,
                        o.OrderProduct?.ProductName,
                        o.OrderClient?.ClientId,
                        o.OrderClient?.ClientName,
                        o.OrderClient?.ClientAddress,
                        o.Quantity,
                        o.Price,
                        o.TotalPrice,
                        o.IsProcesing
                       };

                       orders.Add(oTransform);
                   });


                    string jsonDocument = JsonSerializer.Serialize(orders);

                    StringContent content = new StringContent(jsonDocument, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{solrBaseUrl}/update/json/docs?commit=true", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Ok();
                    }
                    else
                    {
                        return Problem(response.ToString());
                    }


                }

            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {

                string solrBaseUrl = "http://localhost:8983/solr/new_core";
               

                using (HttpClient client = new HttpClient())
                {
                    string query = $"{solrBaseUrl}/select?q=" +
                    $"(OrderId:*{searchTerm}* OR OrderDate:*{searchTerm}*" +
                    $" OR ProductName:*{searchTerm}* OR ClientName:*{searchTerm}*)" +
                    $"&indent=true&useParams=&qt=%2Fselect";

                    HttpResponseMessage response = await client.GetAsync(query);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        return Ok(jsonResponse);
                    }
                    else
                    {
                        return Problem(response.ToString()) ;
                    }
                   
                }

            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }
            
        }
    }
}
