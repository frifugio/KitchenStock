using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using KitchenStock.Shared;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;

namespace KitchenStock.API
{
    public static class StockFunctions
    {
        [FunctionName("GetAllStockItems")]
        public static IActionResult GetAllStockItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<StockItem> stockList, ILogger log)
        {
            return new OkObjectResult(stockList);
        }


        [FunctionName("AddStockItem")]
        public async static Task<IActionResult> AddStockItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]IAsyncCollector<StockItem> stockOut,
            ILogger log)
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                StockItem data = JsonConvert.DeserializeObject<StockItem>(requestBody);
                await stockOut.AddAsync(data);

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new CreatedResult(UriFactory.CreateDocumentUri(databaseName, containerName, data.Id.ToString()), data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("UpdateStockItem")]
        public async static Task<IActionResult> UpdateStockItem(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateStockItem/{id}")]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}")]IAsyncCollector<StockItem> stockOut,
            ILogger log)
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                StockItem data = JsonConvert.DeserializeObject<StockItem>(requestBody);
                await stockOut.AddAsync(data);

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new OkObjectResult(data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("DeleteStockItem")]
        public static async Task<IActionResult> DeleteStockItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteStockItem/{id}")]
            HttpRequest request,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id = {id}")]
            IEnumerable<StockItem> stocks,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]
            DocumentClient documentClient,
            string id,
            ILogger log)
        {
            if (stocks == null || !stocks.Any())
            {
                return new NotFoundResult();
            }

            var stockItem = stocks.First();
            var uri = UriFactory.CreateDocumentUri("%MyDatabase%", "%MyContainer%", id);
            var options = new RequestOptions
            {
                PartitionKey = new PartitionKey(stockItem.Name)
            };

            await documentClient.DeleteDocumentAsync(uri, options);

            return new OkResult();
        }
    }
}
